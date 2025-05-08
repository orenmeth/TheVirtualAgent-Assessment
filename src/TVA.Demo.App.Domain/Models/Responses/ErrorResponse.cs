namespace TVA.Demo.App.Domain.Models.Responses
{
    public class ErrorResponse<T>
    {
        public T? Item { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
