using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("Transport")]
public partial class Transport
{
    [Key]
    public long Id { get; set; }

    public long User { get; set; }

    public bool CanRented { get; set; }

    [StringLength(50)]
    public string Model { get; set; } = null!;

    [StringLength(9)]
    public string Identifier { get; set; } = null!;

    public string? Description { get; set; }

    [Precision(3, 7)]
    public decimal Latitude { get; set; }

    [Precision(3, 7)]
    public decimal Longitude { get; set; }

    [Precision(5, 2)]
    public decimal? MinutePrice { get; set; }

    [Precision(6, 2)]
    public decimal? DayPrice { get; set; }

    public long Owner { get; set; }

    public long Color { get; set; }

    public long Type { get; set; }

    [ForeignKey("Owner")]
    [InverseProperty("TransportOwnerNavigations")]
    public virtual User OwnerNavigation { get; set; } = null!;

    [InverseProperty("TransportNavigation")]
    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    [ForeignKey("Type")]
    [InverseProperty("Transports")]
    public virtual TransportType TypeNavigation { get; set; } = null!;

    [ForeignKey("User")]
    [InverseProperty("TransportUserNavigations")]
    public virtual User UserNavigation { get; set; } = null!;
}
