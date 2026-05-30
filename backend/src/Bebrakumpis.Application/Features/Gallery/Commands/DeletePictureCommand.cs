using Bebrakumpis.Application.Common.CQRS;
using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Interfaces;
using Bebrakumpis.Domain.Interfaces;

namespace Bebrakumpis.Application.Features.Gallery.Commands;

public record DeletePictureCommand(Guid Id) : IRequest<Result>;

public class DeletePictureCommandHandler(
    IPictureRepository pictureRepository,
    IBlobStorageService blobStorageService)
    : IRequestHandler<DeletePictureCommand, Result>
{
    public async Task<Result> HandleAsync(DeletePictureCommand command, CancellationToken ct)
    {
        var picture = await pictureRepository.GetByIdAsync(command.Id, ct);
        if (picture is null)
            return Result.NotFound($"Picture '{command.Id}' not found.");

        await blobStorageService.DeleteByUrlAsync(picture.BlobUrl, ct);
        await pictureRepository.DeleteAsync(command.Id, ct);

        return Result.Success();
    }
}
