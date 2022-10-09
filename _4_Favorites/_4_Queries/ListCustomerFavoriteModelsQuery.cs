using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
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
}

public class ListCustomerFavoriteModelsQueryHandler : IRequestHandler<ListCustomerFavoriteModelsQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public ListCustomerFavoriteModelsQueryHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<CustomerFavoriteModel> logger
    )
    {
        _dbQueryService = dbQueryService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        ListCustomerFavoriteModelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedCustomer = await _dbQueryService.FindAsync<User>(request.CustomerId);
        if(existedCustomer is null)
        {
            _errorMessage = $"Customer Record with Id: {request.CustomerId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var customerFavoriteModels = await _dbQueryService
            .GetQuery<CustomerFavoriteModel>(e => e.CustomerId == request.CustomerId)
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
            })
            .ToListAsync();

        return Results.Ok(customerFavoriteModels);
    }
}
