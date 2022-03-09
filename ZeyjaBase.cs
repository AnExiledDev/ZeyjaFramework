using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Menus;
using Microsoft.Extensions.Logging;
using ZeyjaFramework.Config;

namespace ZeyjaFramework
{
    public class ZeyjaBase
    {
        /// <summary>
        /// Gets the discord client.
        /// </summary>
        public DiscordShardedClient Client { get; }

        /// <summary>
        /// Gets the commands extension.
        /// </summary>
        public IReadOnlyDictionary<int, CommandsNextExtension> Commands { get; set; }

        /// <summary>
        /// Gets the interactivity extension.
        /// </summary>
        public IReadOnlyDictionary<int, InteractivityExtension> Interactivity { get; set; }

        /// <summary>
        /// Gets the database handler.
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeyjaBase"/> class.
        /// </summary>
        /// 
        /// <param name="discordConfiguration">The discord configuration.</param>
        public ZeyjaBase(DiscordConfiguration discordConfiguration, DatabaseConfig databaseConfig)
        {
            Database = new Database(databaseConfig);
            Client = new DiscordShardedClient(discordConfiguration);

            Client.Logger.LogInformation("Initializing client.");

            /*Services = new ServiceCollection()
                .AddSingleton<IDatabase>(new Database())
                .BuildServiceProvider(true);*/
        }

        /// <summary>
        /// Runs the bot.
        /// </summary>
        /// 
        /// <param name="commandsNextConfiguration">The commands next configuration.</param>
        /// <param name="interactivityConfiguration">The interactivity configuration.</param>
        /// <returns>A Task.</returns>
        public async Task RunAsync(CommandsNextConfiguration commandsNextConfiguration,
            InteractivityConfiguration interactivityConfiguration)
        {
            // TODO: Implement MenusConfiguration
            Client.UseMenus();

            Commands = await Client.UseCommandsNextAsync(commandsNextConfiguration);
            Interactivity = await Client.UseInteractivityAsync(interactivityConfiguration);

            Client.Logger.LogInformation("Client is starting.");

            await Client.StartAsync();
        }
    }
}
