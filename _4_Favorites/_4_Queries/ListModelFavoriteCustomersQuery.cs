using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Favorites.Queries;

public class ListModelFavoriteCustomersQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListModelFavoriteCustomersQueryHandler : IRequestHandler<ListModelFavoriteCustomersQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public ListModelFavoriteCustomersQueryHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        ILogger<CustomerFavoriteModel> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ListModelFavoriteCustomersQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedModel = await _dbQueryService.GetOneAsync<Model>(e => e.Id == request.ModelId);
        if(existedModel is null)
        {
            _errorMessage = $"Model Record with Id: {request.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var dbQuery = _dbQueryService
            .GetQuery<CustomerFavoriteModel>(e => e.ModelId == request.ModelId)
            .Include("Customer")
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand")
            .Select(e => new CustomerFavoriteModelDto()
            {
                FullName = $"{e.Customer.FirstName} {e.Customer.LastName}",
                Email = e.Customer.Email,
                BirthDate = e.Customer.BirthDate,
                Gender = e.Customer.Gender,
                Title = e.Model.Title,
                Description = e.Model.Description,
                ThumbUrl = e.Model.ThumbUrl,
                Category = e.Model.Category,
                ProductId = e.Model.ProductId,
                ProductTitle = e.Model.Product.Title,
                BrandId = e.Model.BrandId,
                BrandTitle = e.Model.Brand.Title
            });

        if (request.PageSize is not null && request.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<CustomerFavoriteModelDto>(dbQuery, (int)request.PageSize, (int)request.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            return Results.Ok(list);
        }
    }
}
