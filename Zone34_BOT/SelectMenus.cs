﻿using Discord.WebSocket;
namespace Pale_BOT
{
    enum SelectMenuIds
    {
        ShowPerson,
        ChangePerson,
        RollPerson,
        DeletePerson,
        GivePoints
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
                {SelectMenuIds.DeletePerson.ToString(), new RPSystem.Delete().DeletePerson },
                {SelectMenuIds.GivePoints.ToString(), new RPSystem.Give().GetPersonFromSelectMenu }
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
