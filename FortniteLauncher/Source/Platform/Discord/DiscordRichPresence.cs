using System;
using DiscordRPC;
using System.Threading.Tasks;

class CreativeRPC
{
    private static readonly DiscordRpcClient Client = new("1470924586953478299");
    private static readonly DateTime StartTimestamp = DateTime.UtcNow;

    public static async void Start()
    {
        Client.Initialize();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                UpdatePresence();
                await Task.Delay(1 * 1000);
            }
        });
    }

    private static void UpdatePresence()
    {
        if (!Client.IsInitialized)
            return;

        Client.SetPresence(new RichPresence
        {
            State = "change in discord rich presence",
            Timestamps = new Timestamps { Start = StartTimestamp },

            Assets = new Assets
            {
                LargeImageKey = "fn17",
                LargeImageText = "Creative"
            },

            Buttons = new[]
            {
                new Button { Label = "Join Discord", Url = ProjectDefinitions.Discord },
            }
        });
    }
}
