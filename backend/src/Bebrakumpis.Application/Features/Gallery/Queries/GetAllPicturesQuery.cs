using Bebrakumpis.Application.Common.CQRS;
using Bebrakumpis.Application.Common.Result;
using Bebrakumpis.Application.Features.Gallery.DTOs;
using Bebrakumpis.Domain.Interfaces;

namespace Bebrakumpis.Application.Features.Gallery.Queries;

public record GetAllPicturesQuery : IRequest<Result<IEnumerable<PictureResponse>>>;

public class GetAllPicturesQueryHandler(IPictureRepository pictureRepository)
    : IRequestHandler<GetAllPicturesQuery, Result<IEnumerable<PictureResponse>>>
{
    public async Task<Result<IEnumerable<PictureResponse>>> HandleAsync(GetAllPicturesQuery query, CancellationToken ct)
    {
        var pictures = await pictureRepository.GetAllAsync(ct);
        var response = pictures.Select(p => new PictureResponse
        {
            Id = p.Id,
            BlobUrl = p.BlobUrl,
            Order = p.Order,
            UploadedAt = p.UploadedAt
        });
        return Result<IEnumerable<PictureResponse>>.Success(response);
    }
}
