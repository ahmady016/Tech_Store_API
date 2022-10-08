using System.Net;
using AutoMapper;
using MediatR;

using DB;
using Common;
using Entities;
using Dtos;

namespace Replies.Commands;

public class UpdateReplyCommand : UpdateCommand<AddReplyCommand> {}

public class UpdateReplyCommandHandler : IRequestHandler<UpdateReplyCommand, IResult>
{
    private readonly IDBQueryService _dbQueryService;
    private readonly IDBCommandService _dbCommandService;
    private readonly IMapper _mapper;
    private readonly ILogger<Reply> _logger;
    private string _errorMessage;
    public UpdateReplyCommandHandler(
        IDBQueryService dbQueryService,
        IDBCommandService dbCommandService,
        IMapper mapper,
        ILogger<Reply> logger
    )
    {
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
        var existedReply = await _dbQueryService.FindAsync<Reply>(command.Id);
        if (existedReply is null)
        {
            _errorMessage = $"Reply Record with Id: {command.Id} Not Found";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.NotFound);
        }

        var updatedReply = _mapper.Map<Reply>(command.ModifiedEntity);
        updatedReply.Id = command.Id;

        _dbCommandService.Update<Reply>(updatedReply);
        await _dbCommandService.SaveChangesAsync();

        var ReplyDto = _mapper.Map<ReplyDto>(updatedReply);
        return Results.Ok(ReplyDto);
    }
}
