using RestoreMonarchy.AdvancedEconomy;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;

namespace fr34kyn01535.Uconomy
{
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        public DatabaseManager Database;
        public static Uconomy Instance;

        private AdvancedEconomyPlugin economyPlugin => AdvancedEconomyPlugin.Instance;

        protected override void Load()
        {
            Logger.Log($"Loading UconomyToAdvancedEconomy {Assembly.GetName().Version.ToString(3)}...", ConsoleColor.Yellow);
            
            if (economyPlugin == null || economyPlugin.Events == null)
            {
                Logger.Log("AdvancedEconomy is not loaded!", ConsoleColor.Yellow);
                UnloadPlugin(PluginState.Cancelled);
                return;
            }

            Instance = this;
            Database = new DatabaseManager();
            
            if (Level.isLoaded)
            {
                OnPluginsLoaded();
            } else
            {
                R.Plugins.OnPluginsLoaded += OnPluginsLoaded;
            }

            Logger.Log($"UconomyToAdvancedEconomy {Assembly.GetName().Version.ToString(3)} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;

            if (economyPlugin != null && economyPlugin.Events != null)
            {
                economyPlugin.Events.OnPlayerBalanceChecked -= OnBalanceChecked;
                economyPlugin.Events.OnPlayerBalanceUpdated -= OnPlayerBalanceUpdated;
                economyPlugin.Events.OnPlayerPaid -= OnPlayerPaid;
            }

            Logger.Log($"UconomyToAdvancedEconomy has been unloaded!", ConsoleColor.Yellow);
        }

        private void OnPluginsLoaded()
        {
            if (economyPlugin == null || economyPlugin.Events == null)
            {
                Logger.Log("AdvancedEconomy was not loaded properly!", ConsoleColor.Red);
                UnloadPlugin(PluginState.Cancelled);
                return;
            }

            economyPlugin.Events.OnPlayerBalanceChecked += OnBalanceChecked;
            economyPlugin.Events.OnPlayerBalanceUpdated += OnPlayerBalanceUpdated;
            economyPlugin.Events.OnPlayerPaid += OnPlayerPaid;
        }

        private void OnPlayerPaid(string senderId, string receiverId, decimal amount)
        {
            UnturnedPlayer sender = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(senderId)));
            UnturnedPlayer receiver = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(receiverId)));

            HasBeenPayed(sender, receiver, amount);
        }

        private void OnPlayerBalanceUpdated(string steamId, decimal amount, decimal balance)
        {
            BalanceUpdated(steamId, amount);
        }

        public delegate void PlayerBalanceUpdate(UnturnedPlayer player, decimal amt);
        public event PlayerBalanceUpdate OnBalanceUpdate;
        public delegate void PlayerBalanceCheck(UnturnedPlayer player, decimal balance);
        public event PlayerBalanceCheck OnBalanceCheck;
        public delegate void PlayerPay(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt);
        public event PlayerPay OnPlayerPay;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"command_balance_show","Your current balance is: {0} {1}"},
                    {"command_pay_invalid","Invalid arguments"},
                    {"command_pay_error_pay_self","You cant pay yourself"},
                    {"command_pay_error_invalid_amount","Invalid amount"},
                    {"command_pay_error_cant_afford","Your balance does not allow this payment"},
                    {"command_pay_error_player_not_found","Failed to find player"},
                    {"command_pay_private","You paid {0} {1} {2}"},
                    {"command_pay_console","You received a payment of {0} {1} "},
                    {"command_pay_other_private","You received a payment of {0} {1} from {2}"},
                }; 
            }
        }

        internal void HasBeenPayed(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt)
        {
            if (OnPlayerPay != null)
                OnPlayerPay(sender, receiver, amt);
        }

        internal void BalanceUpdated(string SteamID, decimal amt)
        {
            if (OnBalanceUpdate != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceUpdate(player, amt);
            }
        }

        internal void OnBalanceChecked(string SteamID, decimal balance)
        {
            if (OnBalanceCheck != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceCheck(player, balance);
            }
        }
    }
}
