namespace Dastardly.Infrastructure.Validators;

using Dastardly.Infrastructure.Contracts;
using FastEndpoints;
using FluentValidation;

/// <summary>
/// Validator for the external API CreateOrderRequest
/// </summary>
public sealed class CreateOrderRequestValidator : Validator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEqual(Guid.Empty)
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required")
            .Must(items => items.All(item => !string.IsNullOrWhiteSpace(item)))
            .WithMessage("All items must have valid names");
    }
}
