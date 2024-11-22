using M7_CarManager.Models;
using Microsoft.AspNetCore.SignalR.Client;
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
        HubConnection _hubConnection;
        Random _random;

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

            // configure hub client
            _random = new Random();
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5041/events")
                .Build();
            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(_random.Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            // register for carCreated server side event
            _hubConnection.On<Car>("carCreated", async car => await Refresh());
            // register for carUpdated server side event
            _hubConnection.On<Car>("carUpdated", async car => await Refresh());
            // register for carDeleted server side event
            _hubConnection.On<string>("carDeleted", (id) =>
            {
                var carToDelete = Cars?.FirstOrDefault(c => c.Id == id);
                // only the UI thread can modify the collection to see the modified collection on the GUI
                this.Dispatcher.Invoke(() =>
                {
                    Cars?.Remove(carToDelete);
                });
            });

            // start hub connection
            Task.Run(async () => await _hubConnection.StartAsync());

            // set datacontext of mainwindow
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
            ShowIfError(response);
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.PostAsJsonAsync("/car", ActualCar);
            ShowIfError(response);
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.PutAsJsonAsync("/car", ActualCar);
            ShowIfError(response);
        }

        private async void ShowIfError(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsAsync<ErrorModel>();
                MessageBox.Show(
                    messageBoxText: $"{error.Message} at: {error.Date}",
                    caption: "Error",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
            }
        }
    }
}