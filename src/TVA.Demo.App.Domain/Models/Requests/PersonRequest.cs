namespace TVA.Demo.App.Domain.Models.Requests
{
    public class PersonRequest
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public required string IdNumber { get; set; }
    }
}
