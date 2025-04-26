using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Tourest.Hubs
{
    public class RatingHub : Hub
    {
        
        public async Task JoinTourRatingGroup(string tourId)
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tour-{tourId}");
        }

       
        public async Task LeaveTourRatingGroup(string tourId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tour-{tourId}");
        }

        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
