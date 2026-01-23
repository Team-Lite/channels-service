using FluentResults;

namespace ChannelsService.UseCases;

public interface IHandler<in TCommand>
    where TCommand : Command
{
    public Task<Result> HandleAsync(TCommand command);
}

public interface IHandler<in TCommand, TResult> 
    where TCommand : Command 
    where TResult : class
{
    public Task<Result<TResult>> HandleAsync(TCommand command);
}