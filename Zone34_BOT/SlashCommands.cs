using Amazon.Runtime.Internal.Transform;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using static Pale_BOT.RPSystem.User.Person.Skills;

namespace Pale_BOT
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
                {"create", new RPSystem.CreationCharacter().CreateCharacterAsync},
                {"show", new RPSystem.ShowingCharacter().ShowCharacterAsync },
                {"change", new RPSystem.ChangingCharacter().ChangeCharacterAsync },
                {"roll", new RPSystem.RollPerks().RollingPerksAsync },
                {"delete", new RPSystem.Delete().ChooseDeletePerson },
                {"give", new RPSystem.Give().GivePoints }
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
            try
            {
                List<SlashCommandBuilder> slashCmdList = new List<SlashCommandBuilder>();
                const ulong Zone34_guild = 863762608930160650;
                RoleListCommand(slashCmdList);
                CreateCharacterCommand(slashCmdList);
                ShowPersonInfo(slashCmdList);
                ChangePersonInfo(slashCmdList);
                RollPerk(slashCmdList);
                DeletePerson(slashCmdList);
                GivePoints(slashCmdList);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            command.WithDescription("Позволяет создать персонажа и распределить ему характеристики");
            listCommands.Add(command);
        }

        private void ShowPersonInfo(List<SlashCommandBuilder> listCommands)
        {
            var command = new SlashCommandBuilder()
                .WithName("show").WithDescription("Позволяет увидеть характеристики персонажа выбранного пользователя").AddOption("user", ApplicationCommandOptionType.User, "Пользователь, персонажей которого вы хотите посмотреть", isRequired: true);
            listCommands.Add(command);
        }

        private void ChangePersonInfo(List<SlashCommandBuilder> listCommands)
        {
            var command = new SlashCommandBuilder()
                .WithName("change").WithDescription("Позволяет изменить характеристики персонажа выбранного игрока").AddOption("user", ApplicationCommandOptionType.User, "Пользователь, персонажа которого вы хотите изменить", isRequired: true);
            listCommands.Add(command);
        }

        private void DeletePerson(List<SlashCommandBuilder> listCommands)
        {
            var command = new SlashCommandBuilder()
                .WithName("delete").WithDescription("Позволяет удалить персонажа игрока").AddOption("user", ApplicationCommandOptionType.User, "[ТОЛЬКО ДЛЯ АДМИНИСТРАЦИИ] Позволяет удалить персонажа игрока", isRequired: false);
            listCommands.Add(command);
        }

        private void RollPerk(List<SlashCommandBuilder> listCommands)
        {
            var intellectChoice = new SlashCommandOptionBuilder()
                        .WithName("perk")
                        .WithDescription("Название перка, модификатор которого будет использоваться")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in IntellectSkill.StatDescription)
            {
                intellectChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var intellectOptionBuilder = new SlashCommandOptionBuilder()
                    .WithName(nameof(RPSystem.User.Person.Skills.Intellect).ToLower())
                    .WithDescription("Используются интеллектуальные навыки")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(intellectChoice)
                    .AddOption("modifier", ApplicationCommandOptionType.Integer, "Модификатор броска", isRequired: false);

            var psycheChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true);
            foreach (var statDescr in PsycheSkill.StatDescription)
            {
                psycheChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var psycheOptionBuilder =  new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Psyche).ToLower())
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(psycheChoice)
                .AddOption("modifier", ApplicationCommandOptionType.Integer, "Модификатор броска", isRequired: false);

            var physiqueChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in PhysiqueSkill.StatDescription)
            {
                physiqueChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var physiqueOptionBuilder = new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Physique).ToLower())
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(physiqueChoice)
                .AddOption("modifier", ApplicationCommandOptionType.Integer, "Модификатор броска", isRequired: false);

            var motoricsChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in MotoricsSkill.StatDescription)
            {
                motoricsChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var motoricsOptionBuilder = new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Motorics).ToLower())
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(motoricsChoice)
                .AddOption("modifier", ApplicationCommandOptionType.Integer, "Модификатор броска", isRequired: false);

            SlashCommandBuilder slashCommandBuilder = new SlashCommandBuilder().WithName("roll")
                .WithDescription("Производит бросок двух 6-ти гранных кубов и прибавляет значение выбранного навыка")
                .AddOption(intellectOptionBuilder)
                .AddOption(psycheOptionBuilder)
                .AddOption(physiqueOptionBuilder)
                .AddOption(motoricsOptionBuilder);
            listCommands.Add(slashCommandBuilder);
        }

        private void GivePoints(List<SlashCommandBuilder> listCommands)
        {
            /* Intellect skills option */
            var intellectChoice = new SlashCommandOptionBuilder()
                        .WithName("perk")
                        .WithDescription("Название перка, который будет изменяться")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in IntellectSkill.StatDescription)
            {
                intellectChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var intellectSubcommandBuilder = new SlashCommandOptionBuilder()
                    .WithName(nameof(RPSystem.User.Person.Skills.Intellect).ToLower())
                    .WithDescription("Используются интеллектуальные навыки")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(intellectChoice)
                    .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                    .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значение", isRequired: true);

            /* Psyche skills option */
            var psycheChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, который будет изменяться")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true);
            foreach (var statDescr in PsycheSkill.StatDescription)
            {
                psycheChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var psycheSubcommandBuilder = new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Psyche).ToLower())
                .WithDescription("Используются психические навыки")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(psycheChoice)
                .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значение", isRequired: true);

            /* Physique skills option */
            var physiqueChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, который будет изменяться")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in PhysiqueSkill.StatDescription)
            {
                physiqueChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var physiqueSubcommandBuilder = new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Physique).ToLower())
                .WithDescription("Используются физические навыки")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(physiqueChoice)
                .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значениe", isRequired: true);

            /* Motorics skills option */
            var motoricsChoice = new SlashCommandOptionBuilder()
                .WithName("perk")
                .WithDescription("Название перка, модификатор которого будет использоваться")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String);
            foreach (var statDescr in MotoricsSkill.StatDescription)
            {
                motoricsChoice.AddChoice(statDescr.Key, statDescr.Key);
            }
            var motoricsSubcommandBuilder = new SlashCommandOptionBuilder()
                .WithName(nameof(RPSystem.User.Person.Skills.Motorics).ToLower())
                .WithDescription("Используются навыки моторики")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(motoricsChoice)
                .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значение", isRequired: true);

            /* HP option */
            var HPChoice = new SlashCommandOptionBuilder()
                .WithName("health")
                .WithDescription("Изменение здоровья")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значение", isRequired: true);

            /* Moral option */
            var moralChoice = new SlashCommandOptionBuilder()
                .WithName("moral")
                .WithDescription("Изменение морали")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("user", ApplicationCommandOptionType.User, "Пользователь, которому необходимо изменить значение", isRequired: true)
                .AddOption("value", ApplicationCommandOptionType.Integer, "Добавляемое значение", isRequired: true);

            SlashCommandBuilder slashCommandBuilder = new SlashCommandBuilder().WithName("give")
                .WithDescription("Добавляет (либо вычитает, если отрицательное) значение атрибута")
                .AddOption(intellectSubcommandBuilder)
                .AddOption(psycheSubcommandBuilder)
                .AddOption(physiqueSubcommandBuilder)
                .AddOption(motoricsSubcommandBuilder)
                .AddOption(HPChoice)
                .AddOption(moralChoice);
            listCommands.Add(slashCommandBuilder);
        }
    }
}
