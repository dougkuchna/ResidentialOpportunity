namespace ResidentialOpportunity.Application.DTOs;

public record class WorkCodeDto
{
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;

    public override string ToString() => $"{Code} — {Description}";
}
