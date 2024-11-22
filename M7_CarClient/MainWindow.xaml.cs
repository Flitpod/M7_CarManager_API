using M7_CarManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace M7_CarClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Car> Cars { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Car _actualCar;
        public Car ActualCar
        {
            get
            {
                return _actualCar;
            }
            set
            {
                _actualCar = value?.GetCopy();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualCar)));
            }
        }

        HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5041");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Task.Run(async () =>
            {
                Cars = new ObservableCollection<Car>(await GetCars());
            })
            .Wait();

            this.DataContext = this;
        }

        async Task Refresh()
        {
            Cars = new ObservableCollection<Car>(await GetCars());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cars)));
        }

        async Task<IEnumerable<Car>> GetCars()
        {
            var response = await _httpClient.GetAsync("/car");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Car>>();
            }
            throw new Exception("something went wrong...");
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.DeleteAsync("/car/" + ActualCar.Id);
            response.EnsureSuccessStatusCode();
            await Refresh();
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.PostAsJsonAsync<Car>("/car", ActualCar);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsAsync<ErrorModel>();
                MessageBox.Show(
                    error.Message + " at:" + error.Date.ToShortTimeString(),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await Refresh();
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.PutAsJsonAsync<Car>("/car", ActualCar);
            response.EnsureSuccessStatusCode();
            await Refresh();
        }
    }
}