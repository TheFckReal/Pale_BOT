using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NLog;
using System.Text.Json.Nodes;

namespace Zone34_BOT
{
    internal class Program
    {

        private static MongoClient _mongoClient { get; } = new MongoClient(System.Environment.GetEnvironmentVariable("ConnectionStringMongoDB", System.OperatingSystem.IsLinux() ? EnvironmentVariableTarget.Process : EnvironmentVariableTarget.User));
        public static IMongoDatabase BotDB { get; } = _mongoClient.GetDatabase("DiscordBotData");

        internal static string UserCollectionName { get; } = "UserData";
        private DiscordSocketClient _client { get; set; } = null!;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            Console.WriteLine("App is starting...");
            _client = new DiscordSocketClient(ClientConfigCreate());
            string? token = System.Environment.GetEnvironmentVariable("DiscordToken", System.OperatingSystem.IsLinux() ? EnvironmentVariableTarget.Process : EnvironmentVariableTarget.User);
            Configure();
            await _client.LoginAsync(Discord.TokenType.Bot, token ?? throw new ArgumentNullException("Token", "Token cannot be null"));
            _client.Ready += HandlersConfigure;
            if (_client != null)
            {
                await _client.StartAsync();
                await Task.Delay(-1);
            }
        }

        private void Configure()
        {

        }

        private DiscordSocketConfig ClientConfigCreate()
        {
            DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig();
            discordSocketConfig.GatewayIntents = Discord.GatewayIntents.All;
            return discordSocketConfig;
        }

        private async Task HandlersConfigure()
        {
            await Task.Run(() =>
            {
                var slashCmd = new SlashCommands(client: _client);
                var modalCmd = new Modals(_client);
            });
        }

    }
}