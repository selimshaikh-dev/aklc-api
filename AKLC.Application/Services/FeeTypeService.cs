using AKLC.Application.DTOs.FeeTypes;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class FeeTypeService
        : IFeeTypeService
    {
        private readonly IFeeTypeRepository
            _feeTypeRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public FeeTypeService(
            IFeeTypeRepository feeTypeRepository)
        {
            _feeTypeRepository =
                feeTypeRepository;
        }


        // =========================================
        // GET ALL ACTIVE FEE TYPES
        // =========================================

        public async Task<IReadOnlyList<FeeTypeDto>>
            GetAllActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var feeTypes =
                await _feeTypeRepository
                    .GetAllActiveAsync(
                        cancellationToken);

            return feeTypes
                .Select(
                    MapToDto
                )
                .ToList();
        }


        // =========================================
        // CREATE FEE TYPE
        // =========================================

        public async Task<FeeTypeDto>
            CreateAsync(
                CreateFeeTypeRequest request,
                CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(
                    nameof(request));
            }


            var name =
                request.Name?.Trim()
                ?? string.Empty;


            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Fee type name is required.");
            }


            if (name.Length > 150)
            {
                throw new ArgumentException(
                    "Fee type name cannot exceed 150 characters.");
            }


            var description =
                string.IsNullOrWhiteSpace(
                    request.Description)
                    ? null
                    : request.Description.Trim();


            if (
                description is not null &&
                description.Length > 500
            )
            {
                throw new ArgumentException(
                    "Description cannot exceed 500 characters.");
            }


            var alreadyExists =
                await _feeTypeRepository
                    .ExistsByNameAsync(
                        name,
                        cancellationToken);


            if (alreadyExists)
            {
                throw new InvalidOperationException(
                    "A fee type with this name already exists.");
            }


            var feeType =
                new FeeType
                {
                    Name =
                        name,

                    Description =
                        description,

                    IsActive =
                        request.IsActive,

                    IsDeleted =
                        false
                };


            await _feeTypeRepository
                .AddAsync(
                    feeType,
                    cancellationToken);


            await _feeTypeRepository
                .SaveChangesAsync(
                    cancellationToken);


            return MapToDto(
                feeType);
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static FeeTypeDto
            MapToDto(
                FeeType feeType)
        {
            return new FeeTypeDto
            {
                Id =
                    feeType.Id,

                Name =
                    feeType.Name,

                Description =
                    feeType.Description,

                IsActive =
                    feeType.IsActive,

                CreatedAt =
                    feeType.CreatedAt,

                UpdatedAt =
                    feeType.UpdatedAt
            };
        }
    }
}