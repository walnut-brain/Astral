using Astral;
using Astral.Payloads.DataContracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitLink.Builders;
using RabbitLink.Services.Astral.Adapters;
using RabbitLink.Services.Astral.Descriptions;

namespace RabbitLink.Services.Astral
{
    public static class Extensions
    {
        /// <summary>
        /// Upgrade link builder to service link builder using astral library defaults
        /// </summary>
        /// <param name="linkBuilder">link builder</param>
        /// <param name="settings">json serializer settings, default camel case</param>
        /// <returns>service link builder</returns>
        public static IServiceLinkBuilder UseAstral(this IServiceLinkBuilder linkBuilder, JsonSerializerSettings settings = null)
            => linkBuilder.PayloadManager(new AstralPayloadManager(
                settings == null 
                    ? global::Astral.Payloads.Serialization.Serialization.JsonRaw
                    : global::Astral.Payloads.Serialization.Serialization.MakeJsonRaw(settings),
                new TypeEncoding(
                    TypeEncoder.Default.Fallback(TypeEncoder.KnownType<RpcFail>("rpc.fail"))
                    .Fallback(TypeEncoder.KnownType<RpcOk>("rpc.ok")).Loopback(), 
                    TypeDecoder.Default.Fallback(TypeDecoder.KnownType<RpcFail>("rpc.fail"))
                    .Fallback(TypeDecoder.KnownType<RpcOk>("rpc.ok")).Loopback())))
                .DescriptionFactory(new DescriptionFactory());
    }
}