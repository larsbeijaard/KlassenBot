using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace KlassenBot
{
    class Program
    {
        public static DiscordSocketClient S_SOCKET_CLIENT;
        private CommandService m_Commands;
        private IServiceProvider m_Service;

        static void Main(string[] _args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Setup and run the bot
        /// </summary>
        public async Task RunAsync()
        {
            S_SOCKET_CLIENT = new DiscordSocketClient();
            m_Commands = new CommandService();

            // Make sure there is only one instance of each necessary item
            m_Service = new ServiceCollection()
                .AddSingleton(S_SOCKET_CLIENT)
                .AddSingleton(m_Commands)
                .BuildServiceProvider();

            // Get the bot token from a hidden file, just to be sure ;)
            string _botToken = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "bot-token.tkn");

            S_SOCKET_CLIENT.Log += ClientLog;

            await RegisterCommandAsync();

            // Login and start the bot
            await S_SOCKET_CLIENT.LoginAsync(TokenType.Bot, _botToken);
            await S_SOCKET_CLIENT.StartAsync();

            // Set the bot's playing status
            await S_SOCKET_CLIENT.SetGameAsync("Type !help om van start te gaan!");

            // Make sure the bot doesnt close instantly after start
            await Task.Delay(-1);
        }

        private Task ClientLog(LogMessage _arg)
        {
            Console.WriteLine(_arg);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Let the bot accept commands
        /// </summary>
        public async Task RegisterCommandAsync()
        {
            S_SOCKET_CLIENT.MessageReceived += HandleCommandAsync;

            await m_Commands.AddModulesAsync(Assembly.GetEntryAssembly(), m_Service);
        }

        /// <summary>
        /// Start handling commands when there has been a text message send with the bot its prefix
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage _args)
        {
            var _msg = (SocketUserMessage)_args;
            var _context = new SocketCommandContext(S_SOCKET_CLIENT, _msg);

            // Make sure the bot doesnt reply to itself
            if (_msg.Author.IsBot)
                return;

            // If the message has the correct prefix, execute whatever the command stands for
            int _argPos = 0;
            if (_msg.HasStringPrefix("!", ref _argPos))
            {
                var _result = await m_Commands.ExecuteAsync(_context, _argPos, m_Service);
                if (!_result.IsSuccess)
                    Console.WriteLine(_result.Error);
            }
        }
    }
}
