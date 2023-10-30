using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("User")]
public partial class User
{
    [Key]
    public long Id { get; set; }

    [StringLength(25)]
    public string Username { get; set; } = null!;

    [StringLength(256)]
    public string Password { get; set; } = null!;

    public long Role { get; set; }

    [Precision(6, 2)]
    public decimal Balance { get; set; }

    [InverseProperty("UserNavigation")]
    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    [ForeignKey("Role")]
    [InverseProperty("Users")]
    public virtual Role RoleNavigation { get; set; } = null!;

    [InverseProperty("OwnerNavigation")]
    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
