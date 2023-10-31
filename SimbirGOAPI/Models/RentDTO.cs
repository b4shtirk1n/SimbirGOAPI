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
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        [Required]
        [Precision(6, 2)]
        public decimal PriceOfUnit { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [Precision(7, 2)]
        public decimal Price { get; set; }
    }
}
