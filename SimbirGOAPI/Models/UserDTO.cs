using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class UserDTO
    {
        [MaxLength(25)]
        public string Username { get; set; } = null!;

        [MaxLength(25)]
        public string Password { get; set; } = null!;
    }
}
