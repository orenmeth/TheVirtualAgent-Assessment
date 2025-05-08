namespace TVA.Demo.App.Domain.Models.Responses
{
    public class PersonResponse
    {
        public PersonResponse()
        {
            Accounts = [];
        }

        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public required string IdNumber { get; set; }
        public List<AccountResponse> Accounts { get; set; }
    }
}
