using M7_CarManager.Models;

namespace M7_CarManager.Data
{
    public interface ICarRepository
    {
        void Create(Car car);
        void Delete(string id);
        IEnumerable<Car> Read();
        Car? Read(string id);
        void Update(Car car);
    }
}