using M7_CarClient.Model;
using M7_CarManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace M7_CarManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: "http://www.security.org",
                audience: "http://www.security.org",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256));

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
            });
        }

        [HttpPut]
        public async Task<IActionResult> InsertUser([FromBody] RegisterViewModel model)
        {
            var user = new AppUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhotoContentType = model.PhotoContentType,
                PhotoData = model.PhotoData,
            };

            await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "Customer");
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserInfos()
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhotoContentType = user.PhotoContentType,
                PhotoData = user.PhotoData,
                Roles = await _userManager.GetRolesAsync(user),
            });
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteMyself()
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("[action]")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] RegisterViewModel model)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhotoContentType = model.PhotoContentType;
            user.PhotoData = model.PhotoData;

            if (model.Password != null && model.Password.Length > 0)
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password);
            }
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        // Social Login
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Microsoft([FromBody] SocialToken token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://graph.microsoft.com");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
            var response = await client.GetAsync("/oidc/userinfo");
            MsModel? userInfo = new MsModel();
            if (response.IsSuccessStatusCode)
            {
                userInfo = await response.Content.ReadFromJsonAsync<MsModel>();
                AppUser user = new AppUser
                {
                    FirstName = userInfo.given_name,
                    LastName = userInfo.family_name,
                    Email = userInfo.email,
                    UserName = userInfo.email,
                    EmailConfirmed = true
                };
                return await SocialAuth(user);
            }
            return BadRequest(new ErrorModel() { Message = "Ms login failed" });
        }

        [Route("[action")]
        [HttpPost]
        public async Task<IActionResult> Facebook([FromBody] SocialToken token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://graph.facebook.com");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync($"me?fields=first_name,last_name,picture,email&access_token={token.Token}");
            var result = await response.Content.ReadFromJsonAsync<FbModel>();
            if (result != null)
            {
                AppUser user = new AppUser
                {
                    FirstName = result.last_name,
                    LastName = result.first_name,
                    Email = result.email,
                    UserName = result.email,
                };
                return await SocialAuth(user);
            }
            return Unauthorized();
        }

        [Route("[action")]
        [HttpPost]
        public async Task<IActionResult> Google([FromBody] SocialToken token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://oauth2.googleapis.com");
            client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync($"tokeninfo?id_token={token.Token}");
            var result = await response.Content.ReadFromJsonAsync<GoogleModel>();
            if (result != null)
            {
                AppUser user = new AppUser
                {
                    FirstName = result.given_name,
                    LastName = result.family_name,
                    Email = result.email,
                    UserName = result.email,
                };
                return await SocialAuth(user);
            }
            return Unauthorized();
        }

        private async Task<IActionResult> SocialAuth(AppUser user)
        {
            if (_userManager.Users.FirstOrDefault(t => t.Email == user.Email) == null)
            {
                var res = await _userManager.CreateAsync(user);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
            }
            var appuser = await _userManager.FindByEmailAsync(user.Email);
            if (appuser != null)
            {
                var claim = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub, appuser.UserName),
                    new Claim(JwtRegisteredClaimNames.NameId, appuser.UserName),
                    new Claim(JwtRegisteredClaimNames.Name, appuser.UserName)
                    };
                foreach (var role in await _userManager.GetRolesAsync(appuser))
                {
                    claim.Add(new Claim(ClaimTypes.Role, role));
                }

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
                var token = new JwtSecurityToken(
                    issuer: "http://www.security.org", 
                    audience: "http://www.security.org",
                    claims: claim, 
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
