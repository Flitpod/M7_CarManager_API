using M7_CarManager.Data;
using M7_CarManager.Hubs;
using M7_CarManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace M7_CarManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IHubContext<EventHub> _eventHub;
        private readonly UserManager<AppUser> _userManager;

        public CarController(ICarRepository carRepository, IHubContext<EventHub> eventHub, UserManager<AppUser> userManager)
        {
            _carRepository = carRepository;
            _eventHub = eventHub;
            _userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<Car> GetCars()
        {
            return _carRepository.Read();
        }

        [HttpGet("{id}")]
        public Car? GetCar(string id)
        {
            return _carRepository.Read(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
            car.OwnerId = user?.Id;
            _carRepository.Create(car);
            await _eventHub.Clients.All.SendAsync("carCreated", car);
            return Ok(car);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCar([FromBody] Car car)
        {
            var oldCar = _carRepository.Read(car.Id);
            if (oldCar.Owner.UserName == this.User.Identity.Name)
            {
                _carRepository.Update(car, out oldCar);
                await _eventHub.Clients.All.SendAsync("carUpdated", oldCar);
                return Ok(oldCar);
            }
            else
            {
                throw new Exception("Not your car!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(string id)
        {
            var oldCar = _carRepository.Read(id);
            if (oldCar.Owner.UserName == this.User.Identity.Name)
            {
                _carRepository.Delete(id);
                await _eventHub.Clients.All.SendAsync("carDeleted", id);
                return Ok(id);
            }
            else
            {
                throw new Exception("Not your car!");
            }
        }
    }
}
