using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rcon;

namespace MineCLI;

public class RcServer(RconClient r, JObject s)
{
    readonly RconClient Client = r;
    readonly JObject Server = s;
    public JObject GetServer() => Server;
    public string GetServerProp(string p) => Server[p].ToString();
    public RconClient GetClient() => Client;
}

public static class UserServer
{
    public static async Task<(bool, RcServer)> CreateConnection(JObject serverConfig)
    {
        var client = new RconClient();

        try
        {
            string host = serverConfig["host"]!.ToString();
            string password = serverConfig["password"]!.ToString();
            int port = int.Parse(serverConfig["port"]!.ToString());

            ConsoleWriteColor(ConsoleColor.Yellow, "Connection to the server...");

            var connectTask = client.ConnectAsync(host, port);

            await Task.WhenAny(connectTask, Task.Delay(10000));

            if (!connectTask.IsCompleted)
                throw new TimeoutException("Auth Expired");

            if (!client.Connected)
                throw new Exception("Error with connection of the server");

            var authTask = client.AuthenticateAsync(password);

            await Task.WhenAny(authTask, Task.Delay(5000));

            if (!authTask.IsCompleted)
                throw new TimeoutException("Auth Expired");

            if (client.Authenticated)
            {
                ConsoleWriteColor(ConsoleColor.Green, "Success");
                var server = new RcServer(client, serverConfig);
                return (true, server);
            }
            else
            {
                ConsoleWriteColor(ConsoleColor.Red, "Auth Error");
                return (false, null);
            }
        }
        catch (Exception ex)
        {
            ConsoleWriteColor(ConsoleColor.Red, $"Conn Error {ex.Message}");
            return (false, null);
        }
    }

    public static bool SaveServerConfig(JObject server)
    {
        string filePath = "serverconfig.json";
        try
        {
            File.WriteAllText(
                filePath, 
                JsonConvert.SerializeObject(server)
            );

            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar configuração: {ex.Message}");
            return false;
        }
    }
    
    public static (bool, JObject) GetServerConfigFromMemory(string filePath = "serverconfig.json")
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return (false, []);
            }

            var server = GetJsonFromFile(filePath);

            var host = server["host"].ToString();
            var port = server["port"].ToString();
            var password = server["password"].ToString();

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(port) ||
                string.IsNullOrWhiteSpace(password)
            )
            {
                return (false, []);
            }

            return (true, server);
        }
        catch
        {
            return (false, []);
        }
    }
    
    private static JObject GetJsonFromFile(string filePath) => JObject.Parse(File.ReadAllText(filePath));

    public static void DeleteServerConfig()
    {
        Console.Clear();
        ConsoleWriteColor(ConsoleColor.Yellow, "Are you sure? (Y/N)");

        string response = Console.ReadLine()?.ToUpper() ?? "";

        if (response == "N")
        {
            try
            {
                // Assumindo que existe um arquivo de configuração
                if (File.Exists("serverConfig.json"))
                {
                    File.Delete("serverConfig.json");
                    ConsoleWriteColor(ConsoleColor.Green, "Config deleted successfull");
                }
                else
                {
                    ConsoleWriteColor(ConsoleColor.Yellow, "First you need a configuration.");
                }
            }
            catch (Exception ex)
            {
                ConsoleWriteColor(ConsoleColor.Red, $"Error on config delete: {ex.Message}");
            }
        }
        else
            ConsoleWriteColor(ConsoleColor.Yellow, "Operation Cancelled");

        ConsoleWriteColor(ConsoleColor.Cyan, "Pressione qualquer tecla para continuar...");
        Console.ReadKey(true);
    }
}

public static class MC
{
    public static async Task<(
        bool success, 
        string response, 
        int currentPlayers, 
        int maxPlayers, 
        List<string> playerNames
    )> ConnectAndListPlayersAsync(RcServer sv)
    {
        try
        {
            var client = sv.GetClient();

            string listResponse = await client.SendCommandAsync("list");

            var playerInfo = ParseMinecraftPlayerList(listResponse);

            return (true, listResponse, playerInfo.currentPlayers, playerInfo.maxPlayers, playerInfo.playerNames);
        }
        catch (Exception ex)
        {
            ConsoleWriteColor(ConsoleColor.Red, $"Err: {ex.Message}");
            return (false, ex.Message, 0, 0, new List<string>());
        }
    }

    
    private static (int currentPlayers, int maxPlayers, List<string> playerNames) ParseMinecraftPlayerList(string serverResponse)
    {
        if (string.IsNullOrEmpty(serverResponse))
            return (0, 0, new List<string>());

        var parts = serverResponse.Split(':');
        if (parts.Length < 1)
            return (0, 0, new List<string>());

        var countPart = parts[0].Trim();
        var countMatches = System.Text.RegularExpressions.Regex.Match(countPart, @"There are (\d+) of a max of (\d+) players online");

        if (!countMatches.Success)
            return (0, 0, new List<string>());

        int currentPlayers = int.Parse(countMatches.Groups[1].Value);
        int maxPlayers = int.Parse(countMatches.Groups[2].Value);

        List<string> playerNames = new List<string>();
        if (parts.Length > 1 && currentPlayers > 0)
        {
            string namesSection = parts[1].Trim();
            playerNames = namesSection.Split(',')
                                      .Select(name => name.Trim())
                                      .Where(name => !string.IsNullOrEmpty(name))
                                      .ToList();
        }

        return (currentPlayers, maxPlayers, playerNames);
    }

}




