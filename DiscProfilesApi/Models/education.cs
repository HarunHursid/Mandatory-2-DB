using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class education
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? type { get; set; }

    public int? grade { get; set; }

    [InverseProperty("Education")]
    public virtual ICollection<person> people { get; set; } = new List<person>();
}
