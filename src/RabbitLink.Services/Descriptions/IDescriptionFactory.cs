using System;

namespace RabbitLink.Services.Descriptions
{
    public interface IDescriptionFactory
    {
        ServiceDescription GetDescription(Type type);
    }
}