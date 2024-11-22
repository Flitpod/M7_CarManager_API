using M7_CarManager.Models;

namespace M7_CarManager.Data
{
    public class CarRepository : ICarRepository
    {
        static IList<Car> _cars = new List<Car>()
        {
            new Car { Model = "Peugeot 306", PlateNumber = "ABC-123", Price = 4000 },
            new Car { Model = "Suzuki Swift", PlateNumber = "DCB-321", Price = 1000 },
            new Car { Model = "Opel Astra", PlateNumber = "DDD-333", Price = 2000 },
        };

        public CarRepository() { }

        // CRUD
        public void Create(Car car)
        {
            if (car.PlateNumber.Length != 7)
            {
                throw new ArgumentException("Platenumber format is invalid");
            }
            _cars.Add(car);
        }

        public IEnumerable<Car> Read()
        {
            return _cars;
        }

        public Car? Read(string id)
        {
            return _cars.FirstOrDefault(car => car.Id == id);
        }

        public void Update(Car car)
        {
            var oldCar = IsInCars(car.Id);
            foreach (var prop in typeof(Car).GetProperties().Where(p => p.Name != "Id"))
            {
                prop.SetValue(oldCar, prop.GetValue(car));
            }
        }

        public void Update(Car car, out Car old)
        {
            old = IsInCars(car.Id);
            foreach (var prop in typeof(Car).GetProperties().Where(p => p.Name != "Id"))
            {
                prop.SetValue(old, prop.GetValue(car));
            }
        }

        public void Delete(string id)
        {
            var carToDelete = IsInCars(id);
            _cars.Remove(carToDelete);
        }

        private Car IsInCars(string id)
        {
            var car = _cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                throw new ArgumentException("No car founded!");
            }
            return car;
        }
    }
}
