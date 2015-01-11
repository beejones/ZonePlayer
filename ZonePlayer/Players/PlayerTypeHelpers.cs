//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZonePlayer
{
    /// <summary>
    /// <see cref=" PlayerType" helpers/>
    /// </summary>
    public static class PlayerTypeHelpers
    {
        /// <summary>
        /// Gets or sets all supported player type names
        /// </summary>
        public static List<string> PlayerTypeNames
        {
            get;
            set;
        }

        /// <summary>
        ///  Return all items of <see cref="PlayerType"/>
        /// </summary>
        /// <returns>All items in player type</returns>
        public static List<string> GetPlayerTypes()
        {
            if (PlayerTypeNames != null)
            {
                return PlayerTypeNames;
            }

            var allPlayerTypes = Enum.GetValues(typeof(PlayerType));
            PlayerTypeNames = new List<string>();

            foreach (PlayerType p in allPlayerTypes)
            {
                if (p != PlayerType.None)
                {
                    PlayerTypeNames.Add(p.ToString());
                }
            }

            // Check whether the active X players are installed
            if (new WpfPanel.PanelControl().InitializeVlc() == null)
            {
                // Vlc not installed. Remove activex
                PlayerTypeNames = PlayerTypeNames.Where(t => t != PlayerType.axVlc.ToString()).ToList();
            }

            if (new WpfPanel.PanelControl().InitializeWmp() == null)
            {
                // wmp not installed. Remove activex
                PlayerTypeNames = PlayerTypeNames.Where(t => t != PlayerType.axWmp.ToString()).ToList();
            }

            return PlayerTypeNames;
        }

        /// <summary>
        ///  Convert the string representation of the player to <see cref="PlayerType"/>
        /// </summary>
        /// <param name="playerType">Name of player</param>
        /// <returns>The player type</returns>
        public static PlayerType GetType(string playerType)
        {
            if (string.IsNullOrEmpty(playerType))
            {
                return PlayerType.None;
            }

            playerType = playerType.ToLower();
            List<string> allPlayerTypes = GetPlayerTypes();
            string foundPlayerType = allPlayerTypes.Where(t => t.ToLower().CompareTo(playerType) == 0).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(foundPlayerType))
            {
                return PlayerType.None;
            }

            return (PlayerType)Enum.Parse(typeof(PlayerType), playerType, true);
        }
    }
}
