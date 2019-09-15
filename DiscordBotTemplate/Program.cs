using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBotTemplate.classes;
using Newtonsoft.Json;

namespace DiscordBotTemplate
{
    class Program
    {
        // Create the client
        public static DiscordSocketClient client;

        // Use this class to configure settings on bot startup
        // Keep a private copy ignored by git to keep keys and secrets private
        BotConfig botConfig = new BotConfig();

        // Create a lock to be used with the log file
        public static object writeLock = new object();

        public static void Main()
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient
            (new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            // Set the token to an easy to understand variable
            string botToken = botConfig.Token;

            // Populate the configuration from BotConfig.json for the client to use
            BotConfiguration(ref botConfig);

            // Events to be handled comment out what isn't needed, uncomment what you need
            client.ChannelCreated += ChannelCreated;

            client.Log += Log;

            

            // 

        }

        public static Task Log(LogMessage msg)
        {
            if (!File.Exists("log.txt"))
            {
                File.Create("log.txt");
            }
            lock (writeLock)
            {
                StreamWriter w = File.AppendText("log.txt");
                w.WriteLineAsync($"Log Entry: {DateTime.Now.ToString()}");
                w.WriteLineAsync($"{msg}");
                w.WriteLineAsync("---------------");
                w.Close();
            }
            //Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static void BotConfiguration(ref BotConfig bc)
        {
            JsonTextReader reader;
            try
            {
                // This is good for deployment where I've got the config with the executable
                reader = new JsonTextReader(new StreamReader("json/BotConfig.json"));
                bc = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("json/BotConfig.json"));
            }
            catch (Exception e)
            {
                _ = Log(new LogMessage(LogSeverity.Error, $"BotConfiguration", $"Error reading json/BotConfig.json", e));
            }
        }
    }
}
