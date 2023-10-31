using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class TransportAdminDTO : TransportDTO
    {
        [Required]
        public long Owner { get; set; }
    }
}
