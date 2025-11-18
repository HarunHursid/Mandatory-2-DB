using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("project_id", Name = "idx_tasks_project")]
public partial class task
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; } = null!;

    public int? project_id { get; set; }

    [InverseProperty("task")]
    public virtual ICollection<daily_task_log> daily_task_logs { get; set; } = new List<daily_task_log>();

    [ForeignKey("project_id")]
    [InverseProperty("tasks")]
    public virtual project? project { get; set; }

    [InverseProperty("task")]
    public virtual ICollection<stress_measure> stress_measures { get; set; } = new List<stress_measure>();

    [InverseProperty("task")]
    public virtual ICollection<task_evaluation> task_evaluations { get; set; } = new List<task_evaluation>();

    [ForeignKey("task_id")]
    [InverseProperty("tasks")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
