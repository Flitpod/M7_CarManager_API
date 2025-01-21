namespace M7_CarClient.Model
{
    public class Car
    {
        public string Id { get; set; }
        public string Model { get; set; }
        public string PlateNumber { get; set; }
        public int Price { get; set; }

        public Car()
        {
            Id = Guid.NewGuid().ToString();
        }

        internal Car GetCopy()
        {
            return new Car { Id = Guid.NewGuid().ToString(), Model = Model, PlateNumber = PlateNumber, Price = Price };
        }
    }
}
