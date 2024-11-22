using M7_CarManager.Hubs;
using M7_CarManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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

        IHubContext<EventHub> _eventHub;

        public CarController(IHubContext<EventHub> eventHub)
        {
            _eventHub = eventHub;
        }

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
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            _cars.Add(CarHelper(car));
            await _eventHub.Clients.All.SendAsync("carCreated", car);
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
        public async Task<IActionResult> UpdateCar([FromBody] Car car) 
        {
            var oldCar = GetCar(car.Id);
            if (GetCar(car.Id) == null)
            {
                throw new ArgumentException("Id does not found!");
            }
            foreach (var prop in typeof(Car).GetProperties().Where(p => p.Name != "Id"))
            {
                prop.SetValue(oldCar, prop.GetValue(car));
            }
            await _eventHub.Clients.All.SendAsync("carUpdated", oldCar);
            return Ok(oldCar);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(string id)
        {
            var deleteCar = GetCar(id);
            if (deleteCar == null)
            {
                throw new ArgumentException("Id does not found!");
            }
            _cars.Remove(deleteCar);
            await _eventHub.Clients.All.SendAsync("carDeleted", id);
            return Ok(id);
        }
    }
}
