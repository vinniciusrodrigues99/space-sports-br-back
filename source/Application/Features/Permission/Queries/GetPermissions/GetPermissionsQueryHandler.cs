using FSP.Api.Application.Common.Interfaces;
using FSP.Api.Application.Features.Permission.Queries.GetPermissions;
using FSP.Api.Application.Features.Permission.Queries.GetPermissions.DTOs;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Permission;

namespace FSP.Api.Application.Features.Permission.Queries.GetPermissions
{
    public class GetPermissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPermissionsQuery, ResponseBase<GetPermissionsQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseBase<GetPermissionsQueryResponse>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.GetRepository<Permissao>().AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(p => p.Categoria == request.Category);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => !p.Excluido == request.IsActive.Value);
            }

            // Order by category and name
            var permissions = await query
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nome)
                .ToListAsync(cancellationToken);

            var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

            // Get unique categories from database
            var categories = await _unitOfWork.GetRepository<Permissao>()
                .AsQueryable()
                .Where(p => !p.Excluido)
                .Select(p => p.Categoria)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync(cancellationToken);

            return ResponseBase<GetPermissionsQueryResponse>.Success(new GetPermissionsQueryResponse
            {
                Permissions = permissionDtos,
                Categories = categories!
            });
        }
    }
}
