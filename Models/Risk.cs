using System;
using System.Collections.Generic;

namespace CorporationSecurity.Models;

public partial class Risk
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public int RiskCategoryId { get; set; }

    public string Description { get; set; } = null!;

    public double Likelihood { get; set; }

    public double Impact { get; set; }

    public string? Mitigation { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Asset Asset { get; set; } = null!;

    public virtual ICollection<Control> Controls { get; set; } = new List<Control>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual RiskCategory RiskCategory { get; set; } = null!;
}
