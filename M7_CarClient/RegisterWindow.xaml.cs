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

namespace M7_CarClient.Windows
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            if (tb_password.Password != tb_passwordAgain.Password)
            {
                MessageBox.Show(
                    messageBoxText: "Passwords are not matching!", 
                    caption: "Error", 
                    button: MessageBoxButton.OK, 
                    icon: MessageBoxImage.Error
                );
                return;
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5041/");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PutAsJsonAsync<RegisterViewModel>("auth", new RegisterViewModel()
            {
                Email = tb_Email.Text,
                UserName = tb_userName.Text,
                FirstName = tb_firstName.Text,
                LastName = tb_lastName.Text,
                Password = tb_password.Password,
            });

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show(
                    messageBoxText: "Registration successful",
                    caption: "Info",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Information
                );

                this.DialogResult = true;
            }
        }
    }
}
