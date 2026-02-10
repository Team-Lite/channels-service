using FluentResults;

namespace ChannelsService.Application;

public interface ICommandHandler<in TCommand>
    where TCommand : Command
{
    public Task<Result> HandleAsync(TCommand command);
}

public interface ICommandHandler<in TCommand, TResult> 
    where TCommand : Command 
    where TResult : class
{
    public Task<Result<TResult>> HandleAsync(TCommand command);
}