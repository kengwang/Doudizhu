using System.Reflection;
using Doudizhu.Api.Interfaces.Markers;

namespace Doudizhu.Api.Extensions.DependencyInjection;

public static class DependencyInjectionMarkerExtensions
{
    public static void AddDependencyInjectionMarkerFrom<T>(this IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;
        // find all classes implements
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAbstract || type.IsInterface)
                continue;
            
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType == typeof(IRegisterSelfService))
                {
                    services.AddSingleton(type);
                }
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IRegisterServiceFor<>))
                {
                    services.AddSingleton(interfaceType.GenericTypeArguments[0], type);
                }
            }
        }
    }
}