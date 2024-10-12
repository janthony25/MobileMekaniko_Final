namespace MobileMekaniko_Final.Models.Dto
{
    public class DashboardDto
    {
        public int TotalCustomers { get; set; }
        public int TotalCars { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public int TotalQuotations { get; set; }
        public List<InvoiceListDto> UnpaidInvoices { get; set; }    
    }
}
