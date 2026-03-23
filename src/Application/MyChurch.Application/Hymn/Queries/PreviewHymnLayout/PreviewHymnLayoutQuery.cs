using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Application.Hymn.Services;

namespace MyChurch.Application.Hymn.Queries.PreviewHymnLayout
{
    public class PreviewHymnLayoutQuery : HymnUpsertDto, IRequest<HymnPresentationPreviewDto>
    {
    }

    public class PreviewHymnLayoutQueryHandler : IRequestHandler<PreviewHymnLayoutQuery, HymnPresentationPreviewDto>
    {
        private readonly HymnPresentationPreviewService _previewService;

        public PreviewHymnLayoutQueryHandler(HymnPresentationPreviewService previewService)
        {
            _previewService = previewService;
        }

        public Task<HymnPresentationPreviewDto> Handle(PreviewHymnLayoutQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_previewService.BuildPreview(request));
        }
    }
}
