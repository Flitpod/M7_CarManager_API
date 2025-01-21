using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace M7_CarManager.Models
{
    public class Car
    {
        [Key]
        public string Id { get; set; }
        public string Model { get; set; }
        public string PlateNumber { get; set; }
        public int Price { get; set; }
        public string? OwnerId { get; set; }

        [NotMapped]
        public virtual AppUser? Owner { get; set; }

        public Car()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
