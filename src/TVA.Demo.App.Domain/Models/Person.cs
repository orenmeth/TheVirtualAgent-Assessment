namespace TVA.Demo.App.Domain.Models
{
    public class Person
    {
        public Person()
        {
            Accounts = [];
        }

        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public required string IdNumber { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
