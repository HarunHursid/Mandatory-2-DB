using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("company_id", Name = "idx_departments_company")]
public partial class department
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    public int company_id { get; set; }

    [ForeignKey("company_id")]
    [InverseProperty("departments")]
    public virtual company company { get; set; } = null!;

    [InverseProperty("department")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
