using System.Diagnostics.CodeAnalysis;

namespace TVA.Demo.App.Domain.Entities
{
    public class TransactionDto
    {
        public required int Code { get; set; }
        public required int Account_Code { get; set; }
        public required DateTime Transaction_Date { get; set; }
        public required DateTime Capture_Date { get; set; }
        public required decimal Amount { get; set; }
        public required string Description { get; set; }
    }
}
