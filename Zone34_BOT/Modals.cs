using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pale_BOT
{
    enum ModalIds
    {
        Creation,
        Showing,
        Changing
    }
    internal class Modals
    {
        DiscordSocketClient _client { get; init; }
        public Modals(DiscordSocketClient client)
        {
            _client = client;
            client.ModalSubmitted += ModalHandler;
        }

        delegate Task CallModal(SocketModal socketModal);
        
        private async Task ModalHandler(SocketModal socketModal)
        {
            Dictionary<string, CallModal> LookupModals = new Dictionary<string, CallModal>()
        {
            {ModalIds.Creation.ToString(), new RPSystem.CreationCharacter().GetInfoForCreateAsync},
                {ModalIds.Changing.ToString(), new RPSystem.ChangingCharacter().ChangingDataPersonAsync }
        };

            try
            {
                if (LookupModals.TryGetValue(socketModal.Data.CustomId, out var callModal))
                {
                    //await socketModal.DeferAsync(true);
                    await callModal.Invoke(socketModal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
