using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("EducationID", Name = "idx_persons_education")]
public partial class person
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? private_email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? private_phone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? cpr { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? first_name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? last_name { get; set; }

    public int? experience { get; set; }

    public int? EducationID { get; set; }

    [ForeignKey("EducationID")]
    [InverseProperty("people")]
    public virtual education? Education { get; set; }

    [InverseProperty("person")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();
}
