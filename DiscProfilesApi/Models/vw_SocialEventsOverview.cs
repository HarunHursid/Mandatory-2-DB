using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Keyless]
public partial class vw_SocialEventsOverview
{
    public int social_event_id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string event_name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? event_description { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? company_name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? disc_profile_name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? disc_color { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? disc_description { get; set; }
}
