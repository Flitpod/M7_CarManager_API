using M7_CarManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace M7_CarManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        static IList<Car> _cars = new List<Car>()
        {
            new Car { Model = "Peugeot 306", PlateNumber = "ABC-123", Price = 4000 },
            new Car { Model = "Suzuki Swift", PlateNumber = "DCB-321", Price = 1000 },
            new Car { Model = "Opel Astra", PlateNumber = "DDD-333", Price = 2000 },
        };

        [HttpGet]
        public IEnumerable<Car> GetCars()
        {
            return _cars;
        }

        [HttpGet("{id}")]
        public Car? GetCar(string id)
        {
            return _cars.FirstOrDefault(c => c.Id == id);
        }

        [HttpPost]
        public IActionResult AddCar([FromBody] Car car)
        {
            _cars.Add(CarHelper(car));
            return Ok(car);
        }

        private Car CarHelper(Car car) // would be a logic method, can throw exception
        {
            if(car.PlateNumber.Length != 7)
            {
                throw new ArgumentException("Platenumber format is invalid");
            }
            return car;
        }

        [HttpPut]
        public void UpdateCar([FromBody] Car car) 
        {
            var oldCar = GetCar(car.Id);
            if (oldCar == null)
            {
                throw new Exception("Id does not found!");
            }
            foreach (var prop in typeof(Car).GetProperties().Where(p => p.Name != "Id"))
            {
                prop.SetValue(oldCar, prop.GetValue(car));
            }
        }

        [HttpDelete("{id}")]
        public void DeleteCar(string id)
        {
            var deleteCar = GetCar(id);
            if (deleteCar == null)
            {
                throw new Exception("Id does not found!");
            }
            _cars.Remove(deleteCar);
        }
    }
}
