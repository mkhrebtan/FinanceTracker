namespace Application.Common.Models;

public sealed record TransactionDto
{
    public Guid Id { get; set; }

    required public string TransactionType { get; set; }

    public decimal Amount { get; set; }

    required public string Category { get; set; }

    public DateTime Date { get; set; }

    required public string Description { get; set; }

    required public string Currency { get; set; }
}
