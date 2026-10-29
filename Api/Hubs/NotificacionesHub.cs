using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize]
    public class NotificacionesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Obtén el ID del usuario desde el token JWT (ajusta el claim según tu implementación)
            var userId = Context.User?.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Agrega la conexión a un grupo específico del usuario
                await Groups.AddToGroupAsync(Context.ConnectionId, $"usuario-{userId}");
            }
            await base.OnConnectedAsync();
        }

        // Puedes agregar métodos aquí si necesitas que el cliente invoque algo en el hub
    }
}