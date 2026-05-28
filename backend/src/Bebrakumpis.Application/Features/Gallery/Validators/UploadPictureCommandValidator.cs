using Bebrakumpis.Application.Features.Gallery.Commands;
using FluentValidation;

namespace Bebrakumpis.Application.Features.Gallery.Validators;

public class UploadPictureCommandValidator : AbstractValidator<UploadPictureCommand>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png"];
    private const long MaxFileSize = 10 * 1024 * 1024;

    public UploadPictureCommandValidator()
    {
        RuleFor(x => x.ContentType)
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage("Only JPEG and PNG images are allowed.");
        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File must not be empty.")
            .LessThanOrEqualTo(MaxFileSize).WithMessage("File must not exceed 10 MB.");
    }
}
