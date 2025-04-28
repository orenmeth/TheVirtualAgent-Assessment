namespace TVA.Demo.App.Domain.Models
{
    public class Account
    {
        public Account()
        {
            Transactions = [];
        }
        public int Code { get; set; }
        public int PersonCode { get; set; }
        public required string AccountNumber { get; set; }
        public decimal OutstandingBalance { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
