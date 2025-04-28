namespace TVA.Demo.App.Domain.Models
{
    public class Transaction
    {
        public int Code { get; set; }
        public int AccountCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CaptureDate { get; set; }
        public decimal Amount { get; set; }
        public required string Description { get; set; }
    }
}
