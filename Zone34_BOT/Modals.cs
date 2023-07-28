using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zone34_BOT
{
    enum ModalTypes
    {
        Creation,
        Showing
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
            {ModalTypes.Creation.ToString(), new RPSystem.CreationCharacter().GetInfoForCreate}
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
