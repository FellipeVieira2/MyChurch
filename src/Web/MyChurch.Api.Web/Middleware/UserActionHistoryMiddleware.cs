using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MyChurch.Api.Web.Middleware
{
    public class UserActionHistoryMiddleware
    {
        private readonly RequestDelegate _next;
        private const int MaxBodyLength = 4096; // Limite de 4KB para evitar problemas

        public UserActionHistoryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            string? requestBody = null;
            if (context.Request.ContentLength > 0 && context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    if (requestBody.Length > MaxBodyLength)
                        requestBody = requestBody.Substring(0, MaxBodyLength) + "...";
                    context.Request.Body.Position = 0;
                }
            }

            await _next(context);

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var memberIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(memberIdClaim, out int memberId))
                {
                    var action = context.Request.Path;
                    var method = context.Request.Method;
                    // Monta o objeto de log sem serializar o body novamente
                    var actionDataObj = new {
                        method,
                        path = action.ToString(),
                        body = requestBody
                    };
                    var actionData = JsonSerializer.Serialize(actionDataObj, new JsonSerializerOptions {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });

                    var history = new UserActionHistory
                    {
                        MemberId = memberId,
                        ActionType = action,
                        ActionData = actionData,
                        CreatedAt = DateTime.UtcNow
                    };
                    unitOfWork.UserActionHistories.Create(history);
                    await unitOfWork.CommitAsync();
                }
            }
        }
    }
}
