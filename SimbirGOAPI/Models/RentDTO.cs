using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class RentDTO
    {
        [Required]
        public long Transport { get; set; }

        [Required]
        public long User { get; set; }

        [Required]
        [Precision(5, 0)]
        public TimeOnly Start { get; set; }

        [Precision(5, 0)]
        public TimeOnly End { get; set; }

        [Required]
        [Precision(6, 2)]
        public decimal PriceOfUnit { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [Precision(7, 2)]
        public string Price { get; set; } = null!;
    }
}
