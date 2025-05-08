namespace TVA.Demo.App.Domain.Models.Requests
{
    public class TransactionRequest
    {
        public int Code { get; set; }
        public int AccountCode { get; set; }
        public required string TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public required string Description { get; set; }
    }
}
