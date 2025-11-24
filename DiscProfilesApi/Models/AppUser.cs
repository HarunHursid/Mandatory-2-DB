using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscProfilesApi.Models;

[Index("Email", Name = "UQ_app_users_email", IsUnique = true)]
public partial class AppUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = "user";

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    public int? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("AppUsers")]
    public virtual employee? Employee { get; set; }
}
