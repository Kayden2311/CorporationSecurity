using System;
using System.Collections.Generic;

namespace CorporationSecurity.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
