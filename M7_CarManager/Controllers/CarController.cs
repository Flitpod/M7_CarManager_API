using M7_CarManager.Data;
using M7_CarManager.Hubs;
using M7_CarManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace M7_CarManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        ICarRepository _carRepository;
        IHubContext<EventHub> _eventHub;

        public CarController(ICarRepository carRepository, IHubContext<EventHub> eventHub)
        {
            _carRepository = carRepository;
            _eventHub = eventHub;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<Car> GetCars()
        {
            return _carRepository.Read();
        }

        [HttpGet("{id}")]
        public Car? GetCar(string id)
        {
            return _carRepository.Read(id);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            _carRepository.Create(car);
            await _eventHub.Clients.All.SendAsync("carCreated", car);
            return Ok(car);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCar([FromBody] Car car) 
        {
            _carRepository.Update(car, out Car oldCar);
            await _eventHub.Clients.All.SendAsync("carUpdated", oldCar);
            return Ok(oldCar);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(string id)
        {
            _carRepository.Delete(id);
            await _eventHub.Clients.All.SendAsync("carDeleted", id);
            return Ok(id);
        }
    }
}
