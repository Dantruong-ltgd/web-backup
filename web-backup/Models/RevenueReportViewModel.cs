using System.Collections.Generic;

namespace web_backup.Models
{
    public class RevenueReportViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalPaidInvoices { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public List<MonthlyRevenueItem> MonthlyRevenues { get; set; } = new();
        public List<CategoryRevenueItem> CategoryRevenues { get; set; } = new();
        public List<Invoice> RecentInvoices { get; set; } = new();
    }

    public class MonthlyRevenueItem
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }

    public class CategoryRevenueItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}