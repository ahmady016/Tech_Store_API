using MediatR;
using Microsoft.AspNetCore.Mvc;

using Comments.Queries;
using Replies.Queries;
using Comments.Commands;
using Replies.Commands;

namespace Comments;

[ApiController]
[Route("api/[controller]/[action]")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Comments/ListAll
    /// </summary>
    /// <returns>List of CommentDto</returns>
    [HttpGet]
    public Task<IResult> ListAll(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllCommentsQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Comments/ListModelComments
    /// </summary>
    /// <returns>List of CommentDto</returns>
    [HttpGet]
    public Task<IResult> ListModelComments(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListModelCommentsQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Comments/Search?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of CommentDto</returns>
    [HttpGet]
    public Task<IResult> Search(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchCommentsQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Comments/Find/[id]
    /// </summary>
    /// <returns>CommentDto</returns>
    [HttpGet("{id}")]
    public Task<IResult> Find(Guid id)
    {
        return _mediator.Send(new FindCommentWithRepliesQuery() { Id = id });
    }

    /// <summary>
    /// Comments/ListAllReplies
    /// </summary>
    /// <returns>List of CommentDto</returns>
    [HttpGet]
    public Task<IResult> ListAllReplies(int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new ListAllRepliesQuery() { PageSize = pageSize, PageNumber = pageNumber });
    }
    /// <summary>
    /// Comments/SearchReplies?where=&select=&orderBy=
    /// </summary>
    /// <returns>List of CommentDto</returns>
    [HttpGet]
    public Task<IResult> SearchReplies(string where, string select, string orderBy, int? pageSize, int? pageNumber)
    {
        return _mediator.Send(new SearchRepliesQuery() { Where = where, Select = select, OrderBy = orderBy, PageSize = pageSize, PageNumber = pageNumber });
    }

    /// <summary>
    /// Comments/AddComment
    /// </summary>
    /// <returns>CommentDto</returns>
    [HttpPost]
    public Task<IResult> AddComment(AddCommentCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Comments/UpdateComment
    /// </summary>
    /// <returns>CommentDto</returns>
    [HttpPut]
    public Task<IResult> UpdateComment(UpdateCommentCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Comments/RemoveComment
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> RemoveComment(RemoveCommentCommand command)
    {
        return _mediator.Send(command);
    }

    /// <summary>
    /// Comments/Add
    /// </summary>
    /// <returns>ReplyDto</returns>
    [HttpPost]
    public Task<IResult> AddReply(AddReplyCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Comments/UpdateReply
    /// </summary>
    /// <returns>ReplyDto</returns>
    [HttpPut]
    public Task<IResult> UpdateReply(UpdateReplyCommand command)
    {
        return _mediator.Send(command);
    }
    /// <summary>
    /// Comments/RemoveReply
    /// </summary>
    /// <returns>Message</returns>
    [HttpDelete]
    public Task<IResult> RemoveReply(RemoveReplyCommand command)
    {
        return _mediator.Send(command);
    }

}
