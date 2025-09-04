//using Microsoft.AspNetCore.Mvc;
//using System.Net.WebSockets;
//using System.Text;

//namespace HRMS.Controllers.v1
//{
//    [ApiController]
//    [Route("ws")]
//    public class WebSocketController : ControllerBase
//    {
//        public async Task Get()
//        {
//            if (HttpContext.WebSockets.IsWebSocketRequest)
//            {
//                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
//                await Echo(webSocket);
//            }
//            else
//            {
//                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
//            }
//        }

//        private async Task Echo(WebSocket webSocket)
//        {
//            var buffer = new byte[1024 * 4];
//            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

//            while (!result.CloseStatus.HasValue)
//            {
//                //string message = "hello";
//                var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                var echoBytes = Encoding.UTF8.GetBytes($"Echo: {receivedText}");

//                await webSocket.SendAsync(
//                    new ArraySegment<byte>(echoBytes, 0, echoBytes.Length),
//                    result.MessageType,
//                    result.EndOfMessage,
//                    CancellationToken.None);

//                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//            }

//            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
//        }
//    }
//}
