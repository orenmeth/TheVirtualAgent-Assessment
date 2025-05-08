namespace TVA.Demo.App.Domain.Entities
{
    public class AccountDto
    {
        public required int Code { get; set; }
        public required int Person_Code { get; set; }
        public required string Account_Number { get; set; }
        public required decimal Outstanding_Balance { get; set; }
        public required int Account_Status_Id { get; set; }
    }
}
