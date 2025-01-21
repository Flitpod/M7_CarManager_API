using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Identity.Client;

namespace WpfSocialLogin
{
    /// <summary>
    /// Interaction logic for O365LoginWindow.xaml
    /// </summary>
    public partial class O365LoginWindow : Window
    {
        string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";

 
        string[] scopes = new string[] { "user.read" };

        public string? AccessToken { get; set; }
        public DateTime? TokenExpires { get; set; }

        public O365LoginWindow()
        {
            InitializeComponent();
            Login();
        }

        public async Task Login()
        {
            AuthenticationResult authResult = null;
            var app = App.PublicClientApp;
            var resultText = string.Empty;
            var tokenInfoText = string.Empty;

            var accounts = await app.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    resultText = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                resultText = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                this.Dispatcher.Invoke(() =>
                {
                    this.DialogResult = false;
                });
            }

            if (authResult != null)
            {
                var token = authResult.IdToken;
                resultText = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
                this.AccessToken = token;
                this.TokenExpires = authResult.ExpiresOn.Date;
                this.Dispatcher.Invoke(() =>
                {
                    this.DialogResult = true;
                });
                

            }
        }

        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
