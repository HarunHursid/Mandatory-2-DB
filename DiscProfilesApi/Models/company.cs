using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class company
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? location { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? business_field { get; set; }

    [InverseProperty("company")]
    public virtual ICollection<department> departments { get; set; } = new List<department>();

    [InverseProperty("company")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();

    [InverseProperty("company")]
    public virtual ICollection<social_event> social_events { get; set; } = new List<social_event>();
}
