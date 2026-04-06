namespace iysSite.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public enum CommissionType
{
    Tattoo,
    Shoes
}

public class CommissionRequest : IValidatableObject
{
    [Required]
    public CommissionType CommissionType { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    public ShoeCommissions? Shoes { get; set; }

    public TattooCommissions? Tattoo { get; set; }

    public List<IFormFile> InspirationImages { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CommissionType == CommissionType.Shoes)
        {
            if (Shoes is null)
            {
                yield return new ValidationResult(
                    "Shoe details are required for shoe commissions.",
                    [nameof(Shoes)]);
            }

            if (Tattoo is not null)
            {
                yield return new ValidationResult(
                    "Tattoo details should not be provided for shoe commissions.",
                    [nameof(Tattoo)]);
            }
        }

        if (CommissionType != CommissionType.Tattoo) yield break;
        if (Tattoo is null)
        {
            yield return new ValidationResult(
                "Tattoo details are required for tattoo commissions.",
                [nameof(Tattoo)]);
        }

        if (Shoes is not null)
        {
            yield return new ValidationResult(
                "Shoe details should not be provided for tattoo commissions.",
                [nameof(Shoes)]);
        }
    }
}

public class ShoeCommissions
{
    [Required]
    [MaxLength(100)]
    public string BrandType { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Size { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string CustomStyleColorsIdeas { get; set; } = string.Empty;
}

public class TattooCommissions
{
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Size { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string BodyPlacement { get; set; } = string.Empty;
}