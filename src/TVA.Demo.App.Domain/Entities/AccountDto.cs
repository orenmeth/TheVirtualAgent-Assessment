namespace TVA.Demo.App.Domain.Entities
{
    public class AccountDto
    {
        public required int Code { get; set; }
        public required int PersonCode { get; set; }
        public required string AccountNumber { get; set; }
        public required decimal OutstandingBalance { get; set; }
    }
}
