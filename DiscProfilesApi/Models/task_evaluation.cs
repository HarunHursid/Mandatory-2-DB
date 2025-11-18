using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class task_evaluation
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? description { get; set; }

    public int? difficulty_range { get; set; }

    public int? task_id { get; set; }

    [ForeignKey("task_id")]
    [InverseProperty("task_evaluations")]
    public virtual task? task { get; set; }
}
