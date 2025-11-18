using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

public partial class daily_task_log
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? time_to_finish { get; set; }

    public int? task_id { get; set; }

    [ForeignKey("task_id")]
    [InverseProperty("daily_task_logs")]
    public virtual task? task { get; set; }
}
