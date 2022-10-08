using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;

using DB;
using Dtos;
using Entities;

namespace Replies.Commands;

public class AddReplyCommand : IRequest<IResult>
{
    [Required(ErrorMessage = "Text is Required")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "Text must between 2 and 2000 characters")]
    public string Text { get; set; }

    [Required(ErrorMessage = "CustomerId is Required")]
    [StringLength(450, MinimumLength = 36, ErrorMessage = "CustomerId must between 36 and 450 characters")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "CommentId is Required")]
    [RegularExpression(
        @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$",
        ErrorMessage = "Not a valid CommentId value"
    )]
    public Guid CommentId { get; set; }
}

public class AddReplyCommandHandler : IRequestHandler<AddReplyCommand, IResult>
{
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    public AddReplyCommandHandler(
        IDBCommandService dbCommandService,
        IMapper mapper
    )
    {
        _dbCommandService = dbCommandService;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(
        AddReplyCommand command,
        CancellationToken cancellationToken
    )
    {
        var newReply = _mapper.Map<Reply>(command);
        _dbCommandService.Add<Reply>(newReply);
        await _dbCommandService.SaveChangesAsync();
        var ReplyDto = _mapper.Map<ReplyDto>(newReply);

        return Results.Ok(ReplyDto);
    }
}
