using System;
using System.Collections.Generic;

namespace SimbirGOAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int Role { get; set; }

    public decimal Balance { get; set; }

    public virtual Role? RoleNavigation { get; set; }
}
