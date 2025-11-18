using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class disc_profile
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? color { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? description { get; set; }

    [InverseProperty("disc_profile")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();

    [InverseProperty("disc_profile")]
    public virtual ICollection<projects_disc_profile> projects_disc_profiles { get; set; } = new List<projects_disc_profile>();

    [InverseProperty("disc_profile")]
    public virtual ICollection<social_event> social_events { get; set; } = new List<social_event>();
}
