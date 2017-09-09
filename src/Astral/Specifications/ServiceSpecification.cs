using System;
using System.Linq.Expressions;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Specifications
{
    public abstract class ServiceSpecification : SpecificationBase
    {
        internal ServiceSpecification(LawBook<Fact> lawBook, IServiceProvider provider) : base(lawBook, provider)
        {
        }

        public Type ServiceType => this.GetRequiredService<ServiceType>().Value;
        public string ServiceName => this.GetRequiredService<ServiceName>().Value;

        public EndpointSpecification Endpoint(string name)
        {
            var propertyInfo = ServiceType.GetProperty(name);
            if (propertyInfo == null)
                throw new ArgumentException($"{name} is not valid endpoint property name");
            return Endpoint(propertyInfo);
        }

        protected EndpointSpecification Endpoint(PropertyInfo propertyInfo)
        {
            var book = LawBook.GetOrAddSubBook(propertyInfo.Name, b => b.AddEndpointLaws(propertyInfo)).Result;
            return new EndpointSpecification(book, this);
        }
        
    }

    public class ServiceSpecification<T> : ServiceSpecification
    {
        internal ServiceSpecification(LawBook<Fact> lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
        {
        }

        public EndpointSpecification Endpoint<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector)
            => Endpoint(selector.GetProperty());
            
        
        public EndpointSpecification Endpoint<TArgs>(Expression<Func<T, ICall<TArgs>>> selector)
            => Endpoint(selector.GetProperty());
        
        public EndpointSpecification Endpoint<TArgs, TResult>(Expression<Func<T, ICall<TArgs, TResult>>> selector)
            => Endpoint(selector.GetProperty());
    }
}