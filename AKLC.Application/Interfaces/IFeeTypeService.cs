using AKLC.Application.DTOs.FeeTypes;

namespace AKLC.Application.Interfaces
{
    public interface IFeeTypeService
    {
        Task<IReadOnlyList<FeeTypeDto>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default);

        Task<FeeTypeDto>
            CreateAsync(
                CreateFeeTypeRequest request,
                CancellationToken cancellationToken = default);
    }
}