using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
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
}

public class ListModelFavoriteCustomersQueryHandler : IRequestHandler<ListModelFavoriteCustomersQuery, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerFavoriteModel> _logger;
    private string _errorMessage;
    public ListModelFavoriteCustomersQueryHandler(
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
        ListModelFavoriteCustomersQuery request,
        CancellationToken cancellationToken
    )
    {
        var existedModel = await _dbQueryService
            .GetQuery<Model>(e => e.Id == request.ModelId)
            .Include("Model")
            .Include("Model.Product")
            .Include("Model.Brand")
            .FirstOrDefaultAsync();
        if(existedModel is null)
        {
            _errorMessage = $"Model Record with Id: {request.ModelId} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound( new { Message = _errorMessage });
        }

        var modelFavoriteCustomers = await _dbQueryService
            .GetQuery<CustomerFavoriteModel>(e => e.ModelId == request.ModelId)
            .Include("Customer")
            .ToListAsync();
        modelFavoriteCustomers.ForEach(e => e.Model = existedModel);

        var modelFavoriteCustomersDto = _mapper.Map<List<CustomerFavoriteModelDto>>(modelFavoriteCustomers);
        return Results.Ok(modelFavoriteCustomersDto);
    }
}
