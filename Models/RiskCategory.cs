using System;
using System.Collections.Generic;

namespace CorporationSecurity.Models;

public partial class RiskCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Risk> Risks { get; set; } = new List<Risk>();
}
