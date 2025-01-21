using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace WpfSocialLogin
{
    /// <summary>
    /// Interaction logic for LocalLoginWindow.xaml
    /// </summary>
    public partial class LocalLoginWindow : Window
    {
        public string? AccessToken { get; set; }
        public DateTime? TokenExpires { get; set; }

        string endpoint = "https://localhost:7015/auth";

        public LocalLoginWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(endpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
              new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var request = await client.PostAsJsonAsync("", new
            {
                UserName = tb_username.Text,
                Password = tb_password.Text
            });

            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = await request.Content.ReadAsAsync<TokenModel>();
                this.AccessToken = response.Token;
                this.TokenExpires = response.Expiration;
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }

    class TokenModel
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
