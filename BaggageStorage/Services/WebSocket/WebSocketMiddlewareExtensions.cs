using Microsoft.AspNetCore.Builder;
using BaggageStorage.Services.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaggageStorage.Services.WebSocket
{
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseRtiWebSocket(this IApplicationBuilder builder)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            builder.UseWebSockets(webSocketOptions);

            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
