using Microsoft.AspNetCore.Mvc;
using MediatR;

public static class MapApiRequests
{
  public static WebApplication MapGetRequest<T>(
    this WebApplication app,
    string template
  ) where T : IRequest<IResult>
  {
    app.MapGet(template, async (IMediator mediator, [AsParameters] T request) => await mediator.Send(request));
    return app;
  }

  public static WebApplication MapPostRequest<T>(
  this WebApplication app,
  string template
) where T : IRequest<IResult>
  {
    app.MapPost(template, async (IMediator mediator, [FromBody] T request) => await mediator.Send(request));
    return app;
  }

  public static WebApplication MapPutRequest<T>(
  this WebApplication app,
  string template
) where T : IRequest<IResult>
  {
    app.MapPut(template, async (IMediator mediator, [FromBody] T request) => await mediator.Send(request));
    return app;
  }

  public static WebApplication MapDeleteRequest<T>(
    this WebApplication app,
    string template
  ) where T : IRequest<IResult>
  {
    app.MapDelete(template, async (IMediator mediator, [AsParameters] T request) => await mediator.Send(request));
    return app;
  }

}
