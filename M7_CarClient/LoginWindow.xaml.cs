using M7_CarClient.Model;
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

namespace M7_CarClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5041");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );

            var response = await client.PostAsJsonAsync<LoginViewModel>("auth", new LoginViewModel()
            {
                UserName = tb_username.Text,
                Password = tb_password.Password
            });

            var token = await response.Content.ReadAsAsync<TokenModel>();
            token.Expiration = token.Expiration.ToLocalTime();

            MessageBox.Show(token.Expiration.ToShortTimeString());
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.ShowDialog();
        }
    }
}
