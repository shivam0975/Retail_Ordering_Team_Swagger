using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class OrderStatusHistory
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order? Order { get; set; }
}
