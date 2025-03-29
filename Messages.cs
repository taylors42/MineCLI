using CoreRCON;
using MineCLI;
using Newtonsoft.Json.Linq;

public static class Messages
{
    public static void WelcomeMessage()
    {
        ConsoleWriteColor(ConsoleColor.White, "Welcome to the");
        ConsoleWriteColor(ConsoleColor.Green, "MineCLI");
        Console.WriteLine("\n");
    }

    public static JObject GetServerInformationFromUser()
    {
        var host = GetString("Write the IP or Host of your Minecraft Server:", "Write Something Please");

        var port = GetString("Write the port of your Host (default -> 25575):", "Write Something Please");

        var password = GetString("Write your password:", "Write Something Please");

        var serverObject = new JObject
        {
            ["host"] = host,
            ["port"] = int.Parse(port),
            ["password"] = password,
        };

        return serverObject;
    }

    public static async Task ShowPlayersWithAutoRefresh(RcServer server)
    {
        Console.Clear();
        ConsoleWriteColor(
            ConsoleColor.Yellow,
            "Monitoring online players. Press any key to return for the menu..."
        );
        Console.WriteLine();

        using var refreshCts = new CancellationTokenSource();

        var keyTask = Task.Run(() => {
            Console.ReadKey(true);
            refreshCts.Cancel();
        });

        try
        {

            while (!refreshCts.Token.IsCancellationRequested)
            {
                var (success, response, currentPlayers, maxPlayers, playerNames) =
                    await MC.ConnectAndListPlayersAsync(server)
                    .WaitAsync(TimeSpan.FromSeconds(10), refreshCts.Token);

                if (success)
                {
                    Console.Clear();
                    ConsoleWriteColor(ConsoleColor.Yellow, "Monitoring online players...");
                    Console.WriteLine();
                    ConsoleWriteColor(ConsoleColor.Cyan, $"Players Online:");
                    ConsoleWriteColor(ConsoleColor.Magenta, currentPlayers.ToString());
                    Console.Write("/ ");
                    ConsoleWriteColor(ConsoleColor.Yellow, maxPlayers.ToString());

                    Console.WriteLine();
                    
                    if (playerNames.Count > 0)
                    {
                        ConsoleWriteColor(ConsoleColor.White, "Player list:");
                        Console.WriteLine();
                        foreach (var player in playerNames)
                        {
                            ConsoleWriteColor(ConsoleColor.Green, $"- {player}");
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    ConsoleWriteColor(ConsoleColor.Red, "Err on server.");
                    Console.WriteLine();
                    break;
                }
                
                if (keyTask.IsCompleted)
                {
                    break;
                }
                
                await Task.Delay(10000, refreshCts.Token);
            }
        }
        catch(TaskCanceledException)
        {
        }
    }

    public static async Task ShowServerFunction(RcServer server)
    {
        bool running = true;
        int selectedOption = 0;
        string[] options = { "Show online Players", "Delete my server config file", "Exit" };

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== MINECRAFT SERVER ADMIN ===");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            Console.WriteLine("=================================");
            Console.WriteLine("Use the arrows to navigate and press enter to select");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedOption = (selectedOption > 0) ? selectedOption - 1 : options.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedOption = (selectedOption < options.Length - 1) ? selectedOption + 1 : 0;
                    break;

                case ConsoleKey.Enter:
                    switch (selectedOption)
                    {
                        case 0:
                            await ShowPlayersWithAutoRefresh(server);
                            break;
                        case 1:
                            UserServer.DeleteServerConfig();
                            break;
                        case 2:
                            running = false;
                            Environment.Exit(0);
                            break;
                    }
                    break;
            }
        }
    }
    
    static string GetString(string message, string errorMessage)
    {
        while (true)
        {
            ConsoleWriteColor(ConsoleColor.White, message);
            var result = Console.ReadLine();

            if (string.IsNullOrEmpty(result))
            {
                ConsoleWriteColor(ConsoleColor.Red, errorMessage);
                continue;
            }
            return result;
        }
    }
}