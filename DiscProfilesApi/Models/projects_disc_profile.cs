using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class projects_disc_profile
{
    [Key]
    public int id { get; set; }

    public int project_id { get; set; }

    public int disc_profile_id { get; set; }

    [ForeignKey("disc_profile_id")]
    [InverseProperty("projects_disc_profiles")]
    public virtual disc_profile disc_profile { get; set; } = null!;

    [ForeignKey("project_id")]
    [InverseProperty("projects_disc_profiles")]
    public virtual project project { get; set; } = null!;
}
