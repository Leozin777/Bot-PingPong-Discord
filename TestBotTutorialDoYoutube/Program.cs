using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;


class Program
{
    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    private DiscordSocketClient _client;
    private CommandService _commands;
    private IServiceProvider _serviceProvider;

    public async Task RunBotAsync()
    {
        var configuracoes = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };

        _client = new DiscordSocketClient(configuracoes);
        _commands = new CommandService();

        _serviceProvider = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .BuildServiceProvider();

        string token = "exemplo_token";
        _client.Log += _client_Log;

        await RegisterCommandsAsync();

        await _client.LoginAsync(TokenType.Bot, token);

        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task _client_Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(_client, message);
        if (message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasStringPrefix("!", ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, _serviceProvider);
            if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}



