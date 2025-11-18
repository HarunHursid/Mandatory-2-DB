using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("company_id", Name = "idx_social_events_company")]
[Index("disc_profile_id", Name = "idx_social_events_disc_profile")]
public partial class social_event
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    public int? disc_profile_id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? description { get; set; }

    public int? company_id { get; set; }

    [ForeignKey("company_id")]
    [InverseProperty("social_events")]
    public virtual company? company { get; set; }

    [ForeignKey("disc_profile_id")]
    [InverseProperty("social_events")]
    public virtual disc_profile? disc_profile { get; set; }

    [ForeignKey("social_event_id")]
    [InverseProperty("social_events")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
