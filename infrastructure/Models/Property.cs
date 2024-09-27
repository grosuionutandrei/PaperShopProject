using System;
using System.Collections.Generic;

namespace infrastructure.Models;

public  class Property
{
    public int Id { get; set; }

    public string PropertyName { get; set; } = null!;

    public virtual ICollection<Paper> Papers { get; set; } = new List<Paper>();
}
