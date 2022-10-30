using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;
using Auth;

namespace Replies.Commands;

public class UpdateReplyCommand : UpdateCommand<AddReplyCommand> {}

public class UpdateReplyCommandHandler : IRequestHandler<UpdateReplyCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Reply> _logger;
    private string _errorMessage;
    public UpdateReplyCommandHandler(
        IAuthService authService,
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Reply> logger
    )
    {
        _authService = authService;
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateReplyCommand command,
        CancellationToken cancellationToken
    )
    {
        // get current logged userId from auth
        var userId = _authService.GetCurrentUserId();
        if(userId is null)
        {
            _errorMessage = $"Signin required";
            _logger.LogError(_errorMessage);
            return Results.BadRequest(_errorMessage);
        }

        var existedReply = await _dbQueryService.FindAsync<Reply>(command.Id);
        if (existedReply is null)
        {
            _errorMessage = $"Reply Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound(_errorMessage);
        }

        var updatedReply = _mapper.Map<Reply>(command.ModifiedEntity);
        updatedReply.Id = command.Id;
        updatedReply.CustomerId = userId;

        _dbCommandService.Update<Reply>(updatedReply);
        await _dbCommandService.SaveChangesAsync();

        var ReplyDto = _mapper.Map<ReplyDto>(updatedReply);
        return Results.Ok(ReplyDto);
    }
}
