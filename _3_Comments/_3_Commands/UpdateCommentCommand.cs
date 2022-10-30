using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;
using Auth;

namespace Comments.Commands;

public class UpdateCommentCommand : UpdateCommand<AddCommentCommand> {}

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Comment> _logger;
    private string _errorMessage;
    public UpdateCommentCommandHandler(
        IAuthService authService,
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Comment> logger
    )
    {
        _authService = authService;
        _dbQueryService = dbQueryService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        UpdateCommentCommand command,
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

        var existedComment = await _dbQueryService.FindAsync<Comment>(command.Id);
        if (existedComment is null)
        {
            _errorMessage = $"Comment Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            return Results.NotFound(_errorMessage);
        }

        var updatedComment = _mapper.Map<Comment>(command.ModifiedEntity);
        updatedComment.Id = command.Id;
        updatedComment.CustomerId = userId;

        _dbCommandService.Update<Comment>(updatedComment);
        await _dbCommandService.SaveChangesAsync();

        var commentDto = _mapper.Map<CommentDto>(updatedComment);
        return Results.Ok(commentDto);
    }
}
