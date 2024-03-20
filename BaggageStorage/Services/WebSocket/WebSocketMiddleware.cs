using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BaggageStorage.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BaggageStorage.Classes;


namespace BaggageStorage.Services.WebSocket
{
    public class WebSocketMiddleware
    {
        public static Dictionary<String, WebSocketConnection> WebSocketsList = null;
        public static object SyncRoot = new object(); 

        public class WebSocketConnection
        {
            public System.Net.WebSockets.WebSocket connection { get; set; }
            public CancellationTokenSource tokenSource { get; set; }
        }


        readonly RequestDelegate _next;        

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;            
        }

        public async Task Invoke(HttpContext context)
        {            
            if (context.Request.Path == "/ws" && !Environment.HasShutdownStarted)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var scope = Startup.ServiceProvider.GetService<IServiceScopeFactory>().CreateScope();

                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var _logger = (scope.ServiceProvider.GetRequiredService<ILoggerFactory>()).CreateLogger("RtiWebSocketMiddleware");

                    System.Net.WebSockets.WebSocket webSocket = null;
                    CancellationTokenSource tokenSource = new CancellationTokenSource();

                    try
                    {                        
                        webSocket =await context.WebSockets.AcceptWebSocketAsync();

                        if (!context.User.Identity.IsAuthenticated)
                        {
                            var buffer = Encoding.UTF8.GetBytes("{\"messageType\":-2}");
                            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            string sessionId = context.User.Identity.GetSessionId();

                            lock (SyncRoot)
                            {
                                if (WebSocketsList.ContainsKey(sessionId))
                                    WebSocketsList[sessionId] = new WebSocketConnection { connection = webSocket, tokenSource = tokenSource };
                                else
                                    WebSocketsList.Add(sessionId, new WebSocketConnection { connection = webSocket, tokenSource = tokenSource });
                            }

                            var conn = _db.UserConnection.SingleOrDefault(s => s.SessionId == sessionId);
                            if (conn != null)
                            {
                                conn.IsOnline = true;
                                _db.SaveChanges();
                            }


                            while (webSocket.State == WebSocketState.Open)
                            {
                                var buffer = new ArraySegment<Byte>(new Byte[4096]);
                                var received = await webSocket.ReceiveAsync(buffer, tokenSource.Token);

                                switch (received.MessageType)
                                {
                                    case WebSocketMessageType.Text:
                                        // не будем проверять received.EndOfMessage т.к. знаем, что сообщение не может быть больше 4Кб
                                        var receivedText = Encoding.UTF8.GetString(buffer.Array,
                                                                0, received.Count);

                                        // TODO что либо делаем с полученным сообщением

                                        break;
                                    case WebSocketMessageType.Close:
                                        // обновляем UserConnection
                                        conn = _db.UserConnection.SingleOrDefault(s => s.SessionId == sessionId);
                                        if (conn != null)
                                        {
                                            conn.IsOnline = false;
                                            await _db.SaveChangesAsync();
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogWarning("WS cancel. "+ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(Utils.GetFullError(ex));
                        try
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        }
                        catch { };
                    }

                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

    }
}

