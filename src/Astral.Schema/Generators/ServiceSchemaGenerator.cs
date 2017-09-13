using System;
using System.Reflection;

namespace Astral.Schema.Generators
{
    public class ServiceSchemaGenerator
    {
        public ServiceSchema Generate(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if(!type.IsInterface) throw new ArgumentException($"{nameof(type)} must be an interface");
            var schema = new ServiceSchema();
            var serviceAttr = type.GetCustomAttribute<ServiceAttribute>();
            if(serviceAttr == null)
                throw new InvalidServiceException($"Service {type} don't have service attribute");
            schema.Name = serviceAttr.Name;
            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if(versionAttr == null)
                throw new InvalidServiceException($"Service {type} don't have version attribute");
            schema.Version = versionAttr.Version;
            var className = type.Name;
            if (className.StartsWith("I"))
                className = className.Substring(1);
            schema.Title = className;
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsConstructedGenericType)
                {
                    if (property.PropertyType.GetGenericTypeDefinition() == typeof(IEvent<>))
                    {
                        var eventSchema = new EventSchema();
                        var endpointAttr = property.GetCustomAttribute<EndpointAttribute>();
                        if(endpointAttr == null)
                            throw new InvalidServiceException($"Service {type} event {property.Name} don't have endpoint attribute");
                        eventSchema.Title = property.Name;
                        try
                        {
                            schema.Events.Add(endpointAttr.Name, eventSchema);
                        }
                        catch (ArgumentException aex)
                        {
                            throw new InvalidServiceException($"Service {type} event {property.Name} has invalid endpoint name", aex);
                        }
                    }
                    if (property.PropertyType.GetGenericTypeDefinition() == typeof(ICall<>))
                    {
                        var eventSchema = new EventSchema();
                        var endpointAttr = property.GetCustomAttribute<EndpointAttribute>();
                        if(endpointAttr == null)
                            throw new InvalidServiceException($"Service {type} command {property.Name} don't have endpoint attribute");
                        eventSchema.Title = property.Name;
                        try
                        {
                            schema.Events.Add(endpointAttr.Name, eventSchema);
                        }
                        catch (ArgumentException aex)
                        {
                            throw new InvalidServiceException($"Service {type} command {property.Name} has invalid endpoint name", aex);
                        }
                    }
                }
                    
            }
            return schema;
        }
    }
}