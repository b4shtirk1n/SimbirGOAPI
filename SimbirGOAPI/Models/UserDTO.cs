﻿using System.ComponentModel.DataAnnotations;

namespace SimbirGOAPI.Models
{
    public class UserDTO
    {
        [Required]
        [MaxLength(25)]
        public string Username { get; set; }

        [Required]
        [MaxLength(25)]
        public string Password { get; set; }
    }
}
