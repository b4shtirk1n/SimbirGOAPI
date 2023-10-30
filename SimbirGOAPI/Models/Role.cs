using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("Role")]
public partial class Role
{
    [Key]
    public long Id { get; set; }

    [StringLength(15)]
    public string Name { get; set; } = null!;

    [InverseProperty("RoleNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
