global using static GlobalUtils;
global using static Messages;

namespace MineCLI;
public static class Program
{
    public static async Task Main(string[] args)
    {
        var (result, serverConfig) = UserServer.GetServerConfigFromMemory();
        if (!result)
        {
            while (true)
            {
                WelcomeMessage();

                var serverObject = GetServerInformationFromUser();

                var (connState, svConn) = await UserServer.CreateConnection(serverObject);

                if (connState is false)
                {
                    ConsoleWriteColor(ConsoleColor.Red, "Wrong Password");
                }
                else
                {
                    Console.Clear();
                    ConsoleWriteColor(ConsoleColor.Green, "\nGood Password");
                    Thread.Sleep(1000);
                    bool saveServerStatue = UserServer.SaveServerConfig(svConn.GetServer());
                    return;
                }
            }
        }
        var (connection, server) = await UserServer.CreateConnection(serverConfig);
        if (connection is false)
        {
            ConsoleWriteColor(ConsoleColor.Red, "Connection Error");
            await Main(args);
        }
        ConsoleWriteColor(ConsoleColor.Green, "\nJá temos seu login");
        await ShowServerFunction(server);
    }
}
