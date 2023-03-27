using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVillages.Client.Services
{
    public class MessagesUpdateService
    {

        public event Func<Task> Notify;

        public async Task SendNotificationToUser(ISnackbar Snackbar)
        {
            await RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            if (Notify is { })
            {
                await Notify.Invoke();
            }
        }
    }
}
