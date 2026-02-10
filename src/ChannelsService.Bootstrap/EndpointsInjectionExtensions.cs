using System.Reflection;

namespace ChannelsService.Bootstrap;

public static class EndpointsInjectionExtensions
{
    extension(IEndpointRouteBuilder builder)
    {
        public void UseDefinedEndpoints()
        {
            IEnumerable<Type>? types = Assembly.GetAssembly(typeof(IEndpoint))?
                .GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IEndpoint)) && !x.IsInterface);
            
            foreach (Type type in types ?? [])
            {
                var instance = (IEndpoint)Activator.CreateInstance(type)!;
                instance.Map(builder);
            }
        }
    }
}