using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("TransportType")]
public partial class TransportType
{
    [Key]
    public long Id { get; set; }

    [StringLength(10)]
    public string Name { get; set; } = null!;

    [InverseProperty("TypeNavigation")]
    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
