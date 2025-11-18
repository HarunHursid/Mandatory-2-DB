using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class stress_measure
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? description { get; set; }

    public int? measure { get; set; }

    public int? employee_id { get; set; }

    public int? task_id { get; set; }

    [ForeignKey("employee_id")]
    [InverseProperty("stress_measures")]
    public virtual employee? employee { get; set; }

    [ForeignKey("task_id")]
    [InverseProperty("stress_measures")]
    public virtual task? task { get; set; }
}
