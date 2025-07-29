using System;
using System.Collections.Generic;

namespace CorporationSecurity.Models;

public partial class Control
{
    public int Id { get; set; }

    public int RiskId { get; set; }

    public string Description { get; set; } = null!;

    public string Effectiveness { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Risk Risk { get; set; } = null!;
}
