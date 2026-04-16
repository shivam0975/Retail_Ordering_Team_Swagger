using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? Stock { get; set; }

    public virtual Product? Product { get; set; }
}
