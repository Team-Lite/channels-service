namespace ChannelsService.Application;

public interface IQueryHandler;

public interface IQueryHandler<TResult> : IQueryHandler
{
    Task<TResult> HandleAsync();
}

public interface IQueryHandler<in TQuery, TResult> : IQueryHandler where TQuery : Query
{
    Task<TResult> HandleAsync(TQuery query);
}