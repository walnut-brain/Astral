using Astral.Payloads.DataContracts;
using RabbitLink.Builders;
using RabbitLink.Services.Astral.Adapters;
using RabbitLink.Services.Astral.Descriptions;

namespace RabbitLink.Services.Astral
{
    public static class Extensions
    {
        public static IServiceLinkBuilder UseAstral(this ILinkBuilder linkBuilder, string serviceName)
            => linkBuilder.ToServiceLink(new AstralPayloadManager(
                global::Astral.Payloads.Serialization.Serialization.JsonRaw,
                TypeEncoding.Default), new DescriptionFactory(), serviceName);
    }
}