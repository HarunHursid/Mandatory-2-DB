using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class project
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? description { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? deadline { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? completed { get; set; }

    public int? number_of_employees { get; set; }

    [InverseProperty("project")]
    public virtual ICollection<projects_disc_profile> projects_disc_profiles { get; set; } = new List<projects_disc_profile>();

    [InverseProperty("project")]
    public virtual ICollection<task> tasks { get; set; } = new List<task>();

    [ForeignKey("project_id")]
    [InverseProperty("projects")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
