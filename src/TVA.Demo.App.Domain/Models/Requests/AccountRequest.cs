namespace TVA.Demo.App.Domain.Models.Requests
{
    public class AccountRequest
    {
        public int Code { get; set; }
        public int PersonCode { get; set; }
        public required string AccountNumber { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}
