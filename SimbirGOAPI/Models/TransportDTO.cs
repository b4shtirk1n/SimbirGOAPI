using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class TransportDTO
    {
        [Required]
        public bool CanBeRented { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [StringLength(50)]
        public string Model { get; set; } = null!;

        [Required]
        public string Color { get; set; } = null!;

        [StringLength(9)]
        public string Identifier { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Precision(3, 7)]
        public decimal Latitude { get; set; }

        [Precision(3, 7)]
        public decimal Longitude { get; set; }

        [Precision(5, 2)]
        public decimal MinutePrice { get; set; }

        [Precision(6, 2)]
        public decimal DayPrice { get; set; }
    }
}
