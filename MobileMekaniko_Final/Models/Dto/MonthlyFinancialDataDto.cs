namespace MobileMekaniko_Final.Models.Dto
{
    public class MonthlyFinancialDataDto
    {
        public string MonthName { get; set; }  // e.g., 'January', 'February'
        public decimal TotalInvoicedAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
    }
}
