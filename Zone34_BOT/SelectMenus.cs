using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zone34_BOT
{
    enum SelectMenuIds
    {
        ShowPerson,
        ChangePerson,
        RollPerson,
        DeletePerson
    }
    internal class SelectMenus
    {
        DiscordSocketClient _client { get; init; }
        public SelectMenus(DiscordSocketClient client)
        {
            _client = client;
            client.SelectMenuExecuted += SelectMenuHandler;
        }

        delegate Task CallSelectMenu(SocketMessageComponent component);

        private async Task SelectMenuHandler(SocketMessageComponent component)
        {
            Dictionary<string, CallSelectMenu> LookupSelectMenu = new Dictionary<string, CallSelectMenu>()
            {
                { SelectMenuIds.ShowPerson.ToString(), new RPSystem.ShowingCharacter().ShowingCharacterInfoAsync },
                {SelectMenuIds.ChangePerson.ToString(), new RPSystem.ChangingCharacter().GetInfoForChangingAsync },
                {SelectMenuIds.RollPerson.ToString(), new RPSystem.RollPerks().ChoosePersonAsync },
                {SelectMenuIds.DeletePerson.ToString(), new RPSystem.Delete().DeletePerson }
            };
            if (LookupSelectMenu.TryGetValue(component.Data.CustomId, out CallSelectMenu? callSelectMenu))
            {
                callSelectMenu?.Invoke(component);
            } else
            {
                await component.RespondAsync("Команда не найдена");
            }
        }
    }
}
