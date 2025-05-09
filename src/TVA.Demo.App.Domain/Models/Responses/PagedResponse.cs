namespace TVA.Demo.App.Domain.Models.Responses
{
    public struct PagedResponse<T>
    {
        public PagedResponse()
        {
            Items = [];
        }

        public List<T> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public readonly int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}