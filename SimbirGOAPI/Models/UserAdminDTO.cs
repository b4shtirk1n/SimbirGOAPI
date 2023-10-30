using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class UserAdminDTO : UserDTO
    {
        [Required]
        public bool IsAdmin { get; set; }

        [Precision(6, 2)]
        public decimal Balance { get; set; }
    }
}
