namespace TVA.Demo.App.Domain.Entities
{
    public class PersonDto
    {
        public required int Code { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public required string Id_Number { get; set; }
    }
}
    