using Amazon.Runtime.Internal.Transform;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Zone34_BOT
{
    internal class SlashCommands
    {
        delegate Task CallCommand(SocketSlashCommand command);

        DiscordSocketClient _client { get; init; }
        public SlashCommands(DiscordSocketClient client)
        {
            this._client = client;
            client.SlashCommandExecuted += SlashCommandHandler;
            _ = CreateRPSystemCommands();
        }

        internal async Task SlashCommandHandler(SocketSlashCommand command)
        {
            CallCommand? callCommand = null;
            Dictionary<string, CallCommand> LookupCommands = new Dictionary<string, CallCommand>()
        {
            {"listperks", new RPSystem.RoleList().ShowRoleList},
            {"create", new RPSystem.CreationCharacter().CreateCharacter},
            {"show", new RPSystem.ShowingCharacter().ShowCharacter }
        };
            try
            {
                if (LookupCommands.TryGetValue(command.CommandName, out callCommand))
                {
                    await callCommand.Invoke(command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + $" {ex.StackTrace}");
            }
        }


        internal async Task CreateRPSystemCommands()
        {
            List<SlashCommandBuilder> slashCmdList = new List<SlashCommandBuilder>();
            const ulong Zone34_guild = 863762608930160650;
            RoleListCommand(slashCmdList);
            CreateCharacterCommand(slashCmdList);
            ShowPersonInfo(slashCmdList);
            try
            {
                foreach (var slashCmd in slashCmdList)
                {
                    await _client.Rest.CreateGuildCommand(slashCmd.Build(), Zone34_guild);
                }
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine("CreateRP class\n" + json);
            }
        }

        private void RoleListCommand(List<SlashCommandBuilder> listCommands)
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            command.WithName("listperks");
            command.WithDescription("Команда вывода описаний навыков в выбранной характеристике");
            command.AddOption(new SlashCommandOptionBuilder()
                .WithName("attribute")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithDescription("Характеристика для группы навыков")
                .WithRequired(true)
                .AddChoice("Интеллект", ((int)PerksId.Intelligence))
                .AddChoice("Психика", ((int)PerksId.Mentality))
                .AddChoice("Физика", ((int)PerksId.Physic))
                .AddChoice("Моторика", ((int)PerksId.Motility))
                .AddChoice("Все", ((int)PerksId.All)));
            listCommands.Add(command);
        }

        private void CreateCharacterCommand(List<SlashCommandBuilder> listCommands)
        {
            var command = new SlashCommandBuilder();
            command.WithName("create");
            command.WithDescription("Команда позволяет создать персонажа и распределить ему характеристики");
            listCommands.Add(command);
        }

        private void ShowPersonInfo(List<SlashCommandBuilder> listCommands)
        {
            var command = new SlashCommandBuilder()
                .WithName("show").WithDescription("Команда позволяет увидеть характеристики персонажа выбранного пользователя").AddOption("user", ApplicationCommandOptionType.User, "Пользователь, персонажей которого вы хотите посмотреть", isRequired: true);
            listCommands.Add(command);
        }
    }
}
