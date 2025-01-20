using M7_CarClient.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        RegisterViewModel _registerViewModel = new RegisterViewModel();

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

            _registerViewModel.Email = tb_Email.Text;
            _registerViewModel.UserName = tb_userName.Text;
            _registerViewModel.FirstName = tb_firstName.Text;
            _registerViewModel.LastName = tb_lastName.Text;
            _registerViewModel.Password = tb_password.Password;
            var response = await client.PutAsJsonAsync<RegisterViewModel>("auth", _registerViewModel);

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

        private void Button_UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                byte[] photoData = File.ReadAllBytes(filename);
                string contentType = MimeMapping.MimeUtility.GetMimeMapping(filename);
                img.Source = ToImage(photoData);
                _registerViewModel.PhotoContentType = contentType;
                _registerViewModel.PhotoData = photoData;
            }
        }

        public BitmapImage ToImage(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
                return image;
            }
        }
    }
}
