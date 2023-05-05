using Discord.Commands;

namespace TestBotTutorialDoYoutube.Modules;

public class Command : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task Ping()
    {
        await ReplyAsync("Pong");
    }
}
