using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("Rent")]
public partial class Rent
{
    [Key]
    public long Id { get; set; }

    public long Transport { get; set; }

    public long User { get; set; }

    [Precision(5, 0)]
    public TimeOnly TimeStart { get; set; }

    [Precision(5, 0)]
    public TimeOnly? TimeEnd { get; set; }

    [Precision(7, 2)]
    public decimal? Price { get; set; }

    public long Type { get; set; }

    [ForeignKey("Transport")]
    [InverseProperty("Rents")]
    public virtual Transport TransportNavigation { get; set; } = null!;

    [ForeignKey("Type")]
    [InverseProperty("Rents")]
    public virtual RentType TypeNavigation { get; set; } = null!;

    [ForeignKey("User")]
    [InverseProperty("Rents")]
    public virtual User UserNavigation { get; set; } = null!;
}
