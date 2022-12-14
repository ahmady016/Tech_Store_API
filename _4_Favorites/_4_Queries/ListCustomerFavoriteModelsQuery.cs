using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MediatR;

using DB;
using Entities;
using Dtos;

namespace Favorites.Queries;

public class ListCustomerFavoriteModelsQuery : IRequest<IResult>
{
    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 36, ErrorMessage = "CustomerId must between 36 and 450 characters")]
    public string CustomerId { get; set; }
    public int? PageSize { get; set; } = null;
    public int? PageNumber { get; set; } = null;
}

public class ListCustomerFavoriteModelsQueryHandler : IRequestHandler<ListCustomerFavoriteModelsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public ListCustomerFavoriteModelsQueryHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        ILogger<CustomerFavoriteModel> logger
    )
    {
        _dbQueryService = dbQueryService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ListCustomerFavoriteModelsQuery query,
        CancellationToken cancellationToken
    )
    {
        var existedCustomer = await _dbQueryService.FindAsync<User>(query.CustomerId);
        if(existedCustomer is null)
        {
            _errorMessage = $"Customer Record with Id: {query.CustomerId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var dbQuery = _dbQueryService
            .GetQuery<CustomerFavoriteModel>(e => e.CustomerId == query.CustomerId)
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

        if (query.PageSize is not null && query.PageNumber is not null)
        {
            var page = await _dbQueryService.GetPageAsync<CustomerFavoriteModelDto>(dbQuery, (int)query.PageSize, (int)query.PageNumber);
            return Results.Ok(page);
        }
        else
        {
            var list = await dbQuery.ToListAsync();
            return Results.Ok(list);
        }
    }
}
