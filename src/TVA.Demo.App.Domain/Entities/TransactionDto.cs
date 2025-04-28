namespace TVA.Demo.App.Domain.Entities
{
    public class TransactionDto
    {
        public required int Code { get; set; }
        public required int AccountCode { get; set; }
        public required DateTime TransactionDate { get; set; }
        public required DateTime CaptureDate { get; set; }
        public required decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
