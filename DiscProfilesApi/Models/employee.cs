using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("company_id", Name = "idx_employees_company")]
[Index("department_id", Name = "idx_employees_department")]
[Index("person_id", Name = "idx_employees_person")]
public partial class employee
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? phone { get; set; }

    public int company_id { get; set; }

    public int? person_id { get; set; }

    public int? department_id { get; set; }

    public int? position_id { get; set; }

    public int? disc_profile_id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastLogin { get; set; }

    [ForeignKey("company_id")]
    [InverseProperty("employees")]
    public virtual company company { get; set; } = null!;

    [ForeignKey("department_id")]
    [InverseProperty("employees")]
    public virtual department? department { get; set; }

    [ForeignKey("disc_profile_id")]
    [InverseProperty("employees")]
    public virtual disc_profile? disc_profile { get; set; }

    [ForeignKey("person_id")]
    [InverseProperty("employees")]
    public virtual person? person { get; set; }

    [ForeignKey("position_id")]
    [InverseProperty("employees")]
    public virtual position? position { get; set; }

    [InverseProperty("employee")]
    public virtual ICollection<stress_measure> stress_measures { get; set; } = new List<stress_measure>();

    [ForeignKey("employee_id")]
    [InverseProperty("employees")]
    public virtual ICollection<project> projects { get; set; } = new List<project>();

    [ForeignKey("employee_id")]
    [InverseProperty("employees")]
    public virtual ICollection<social_event> social_events { get; set; } = new List<social_event>();

    [ForeignKey("employee_id")]
    [InverseProperty("employees")]
    public virtual ICollection<task> tasks { get; set; } = new List<task>();

    [InverseProperty("Employee")]
    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
}
