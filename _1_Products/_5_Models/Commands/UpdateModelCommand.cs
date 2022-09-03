using System.Net;
using MediatR;

using DB;
using Entities;
using Dtos;
using Common;

namespace Models.Commands;

public class UpdateModelCommand : UpdateCommand<AddModelCommand> {}

public class UpdateModelCommandHandler : IRequestHandler<UpdateModelCommand, IResult> {
    private readonly IDBService _dbService;
    private readonly ICrudService _crudService;
    private readonly ILogger<Model> _logger;
    private string _errorMessage;
    public UpdateModelCommandHandler(
        IDBService dbService,
        ICrudService crudService,
        ILogger<Model> logger
    )
    {
        _dbService = dbService;
        _crudService = crudService;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateModelCommand command,
        CancellationToken cancellationToken
    )
    {
        // get existed db item
        var oldModel = _crudService.GetById<Model>(command.Id);
        // if title changed
        if(oldModel.Title != command.ModifiedEntity.Title)
        {
            // check if the title are existed in db then reject the command and return error
            var modelWithSameTitle = _dbService.GetOne<Model>(e => e.Title == command.ModifiedEntity.Title);
            if (modelWithSameTitle is not null)
            {
                _errorMessage = $"Model Title is already existed.";
                _logger.LogError(_errorMessage);
                throw new HttpRequestException(_errorMessage, null, HttpStatusCode.Conflict);
            }
        }

        // do the normal update action
        var updatedModel = _crudService.Update<Model, ModelDto, UpdateModelCommand, AddModelCommand>(command, oldModel);
        return await Task.FromResult(Results.Ok(updatedModel));
    }

}
