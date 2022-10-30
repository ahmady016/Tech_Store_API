using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;
using Auth;

namespace Comments.Commands;

public class AddCommentCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Text is Required")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "Text must between 2 and 2000 characters")]
    public string Text { get; set; }

    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, IResult>
{
    private readonly IAuthService _authService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Comment> _logger;
    private string _errorMessage;

    public AddCommentCommandHandler(
        IAuthService authService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Comment> logger
    )
    {
        _authService = authService;
        _dbCommandService = dbCommandService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IResult> Handle(
        AddCommentCommand command,
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

        // create the new Comment and save it to db
        var newComment = _mapper.Map<Comment>(command);
        newComment.CustomerId = userId;
        _dbCommandService.Add<Comment>(newComment);
        await _dbCommandService.SaveChangesAsync();

        var commentDto = _mapper.Map<CommentDto>(newComment);
        return Results.Ok(commentDto);
    }
}
