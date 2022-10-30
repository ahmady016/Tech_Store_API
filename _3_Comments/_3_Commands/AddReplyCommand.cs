using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Replies.Commands;

public class AddReplyCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Text is Required")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "Text must between 2 and 2000 characters")]
    public string Text { get; set; }

    [Required(ErrorMessage = "CommentId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid CommentId value"
    )]
    public Guid CommentId { get; set; }
}

public class AddReplyCommandHandler : IRequestHandler<AddReplyCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Reply> _logger;
    private string _errorMessage;

    public AddReplyCommandHandler(
        IAuthService authService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Reply> logger
    )
    {
        _authService = authService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddReplyCommand command,
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

        var newReply = _mapper.Map<Reply>(command);
        newReply.CustomerId = userId;

        // create the new Reply and save it to db
        _dbCommandService.Add<Reply>(newReply);
        await _dbCommandService.SaveChangesAsync();

        var ReplyDto = _mapper.Map<ReplyDto>(newReply);
        return Results.Ok(ReplyDto);
    }
}
