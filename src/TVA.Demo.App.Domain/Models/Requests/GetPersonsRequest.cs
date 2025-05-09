using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TVA.Demo.App.Domain.Models.Requests
{
    public class GetPersonsRequest
    {
        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; } = nameof(PersonRequest.Code);

        [FromQuery(Name = "filter")]
        public string? Filter { get; set; }

        [FromQuery(Name = "page")]
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "descending")]
        public bool Descending { get; set; } = false;
    }
}
