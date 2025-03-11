using RestoreMonarchy.AdvancedEconomy;
using Steamworks;
using System;

namespace fr34kyn01535.Uconomy
{
    public class DatabaseManager
    {
        private AdvancedEconomyPlugin economyPlugin => AdvancedEconomyPlugin.Instance;

        /// <summary>
        /// returns the current balance of an account
        /// </summary>
        /// <param name="steamId"></param>
        /// <returns></returns>
        public decimal GetBalance(string id)
        {
            return economyPlugin.Database.GetPlayerBalance(id);
        }

        /// <summary>
        /// Increasing balance to increaseBy (can be negative)
        /// </summary>
        /// <param name="steamId">steamid of the accountowner</param>
        /// <param name="increaseBy">amount to change</param>
        /// <returns>the new balance</returns>
        public decimal IncreaseBalance(string id, decimal increaseBy)
        {
            return economyPlugin.Database.IncreasePlayerBalance(id, increaseBy, DateTime.UtcNow);
        }
        
        public void CheckSetupAccount(CSteamID id)
        {

        }
    }
}
