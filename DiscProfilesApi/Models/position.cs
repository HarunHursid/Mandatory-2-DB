using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class position
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [InverseProperty("position")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
