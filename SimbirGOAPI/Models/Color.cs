using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

[Table("Color")]
public partial class Color
{
    [Key]
    public long Id { get; set; }

    [StringLength(10)]
    public string Name { get; set; } = null!;
}
