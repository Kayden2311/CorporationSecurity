using System;
using System.Collections.Generic;

namespace CorporationSecurity.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Control> Controls { get; set; } = new List<Control>();

    public virtual ICollection<Risk> Risks { get; set; } = new List<Risk>();

    public virtual Role Role { get; set; } = null!;
}
