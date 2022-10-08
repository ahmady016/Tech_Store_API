using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Comments.Commands;

public class AddCommentCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Text is Required")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "Text must between 2 and 2000 characters")]
    public string Text { get; set; }

    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 36, ErrorMessage = "CustomerId must between 36 and 450 characters")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "ModelId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid ModelId value"
    )]
    public Guid ModelId { get; set; }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, IResult>
{
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    public AddCommentCommandHandler(
        IDBCommandService dbCommandService,
        IMapper mapper
    )
    {
        _dbCommandService = dbCommandService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken
    )
    {
        var newComment = _mapper.Map<Comment>(command);
        _dbCommandService.Add<Comment>(newComment);
        await _dbCommandService.SaveChangesAsync();
        var commentDto = _mapper.Map<CommentDto>(newComment);

        return Results.Ok(commentDto);
    }
}
