﻿namespace M7_CarManager.Models
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
            return new Car { Id = Id, Model = Model, PlateNumber = PlateNumber, Price = Price }; 
        }
    }
}