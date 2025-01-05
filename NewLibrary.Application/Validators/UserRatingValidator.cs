using FluentValidation;
using NewLibrary.Core.Entities;

public class UserRatingValidator : AbstractValidator<UserRating>
{
    public UserRatingValidator()
    {
        RuleFor(ur => ur.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");
    }
}
