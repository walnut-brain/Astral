using Astral.Payloads.DataContracts;
using Microsoft.Extensions.Logging;
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

        public static ILinkBuilder LoggerFactory(this ILinkBuilder builder, ILoggerFactory factory)
            => builder.LoggerFactory(new LoggerFactoryAdapter(factory));

        public static IServiceLinkBuilder LoggerFactory(this IServiceLinkBuilder buiilder, ILoggerFactory factory)
            => buiilder.LoggerFactory(new LoggerFactoryAdapter(factory));
    }
}