using M7_CarManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                _actualCar = value.GetCopy(); 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualCar)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Cars = new ObservableCollection<Car>();
            Cars.Add(new Car { Model = "Peugeot 406", PlateNumber = "ABC-123", Price = 6000 });
            Cars.Add(new Car { Model = "Peugeot 306", PlateNumber = "HFG-556", Price = 3000 });
            Cars.Add(new Car { Model = "Peugeot 3008", PlateNumber = "TZH-256", Price = 16000 });
            this.DataContext = this;
        }
    }
}