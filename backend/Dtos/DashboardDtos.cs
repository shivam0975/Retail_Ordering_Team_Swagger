namespace backend.Dtos;

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }

    public int TotalOrders { get; set; }

    public int ActiveUsers { get; set; }

    public decimal Revenue { get; set; }

    public int PendingOrders { get; set; }
}
