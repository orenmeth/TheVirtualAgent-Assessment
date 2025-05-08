using TVA.Demo.App.Domain.Models.Requests;

namespace TVA.Demo.App.Domain.Models.Responses
{
    public class AccountResponse
    {
        public AccountResponse()
        {
            Transactions = [];
        }
        public int Code { get; set; }
        public int PersonCode { get; set; }
        public required string AccountNumber { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int AccountStatusId { get; set; }
        public List<TransactionResponse> Transactions { get; set; }
    }
}
