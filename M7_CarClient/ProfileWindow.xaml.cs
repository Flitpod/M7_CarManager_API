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
using System.Windows.Threading;

namespace M7_CarClient
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        // members
        private HttpClient _httpClient;
        private UserInfo _userInfo;

        // ctor
        public ProfileWindow(TokenModel token)
        {
            InitializeComponent();
            HttpClient_Init(token);
            Load_Profile();
        }

        // init methods
        private void HttpClient_Init(TokenModel tokenModel)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5041/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.Token);
        }

        private void Load_Profile()
        {
            Task.Run(async () =>
            {
                _userInfo = await GetUserInfo();
            }).Wait();

            tb_firstName.Text = _userInfo.FirstName;
            tb_lastName.Text = _userInfo.LastName;
            tb_Email.Text = _userInfo.Email;
            tb_userName.Text = _userInfo.UserName;
            if (_userInfo.PhotoData != null)
            {
                img.Source = ToImage(_userInfo.PhotoData);
            }
        }

        private async Task<UserInfo> GetUserInfo()
        {
            var response = await _httpClient.GetAsync("auth");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<UserInfo>();
            }
            throw new Exception("Somewthing went wrong...");
        }
        private ImageSource ToImage(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        // handlers
        private async void Button_UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.FirstName = tb_firstName.Text;
            registerViewModel.LastName = tb_lastName.Text;
            registerViewModel.UserName = tb_userName.Text;
            registerViewModel.Email = tb_Email.Text;
            registerViewModel.Password = tb_password.Password;
            registerViewModel.PhotoData = _userInfo.PhotoData;
            registerViewModel.PhotoContentType = _userInfo.PhotoContentType;

            var response = await _httpClient.PostAsJsonAsync<RegisterViewModel>("auth/update", registerViewModel);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Update successful");
                this.DialogResult = true;
            }
        }

        private void Button_UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "png files (*.png)|*.png|jpeg files (*.jped)|*.jepg";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                _userInfo.PhotoData = File.ReadAllBytes(filename);
                _userInfo.PhotoContentType = MimeMapping.MimeUtility.GetMimeMapping(filename);
                img.Source = ToImage(_userInfo.PhotoData);
            }
        }
    }
}
