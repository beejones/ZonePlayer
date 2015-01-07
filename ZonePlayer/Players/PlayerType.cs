//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayer
{
    /// <summary>
    /// Enumeration of the supported players
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// wmp player
        /// </summary>
        wmp,

        /// <summary>
        /// wmp player activeX control
        /// </summary>
        axWmp,

        /// <summary>
        /// vlc player
        /// </summary>
        vlc,

        /// <summary>
        /// vlc player activeX control
        /// </summary>
        axVlc,

        /// <summary>
        /// No player specified
        /// </summary>
        None
    }

    /// <summary>
    /// <see cref=" PlayerType" helpers/>
    /// </summary>
    public static class PlayerTypeHelper
    {
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
            string[] allPlayerTypes = (string[])Enum.GetValues(typeof(PlayerType));
            string foundPlayerType = allPlayerTypes.Where(t => t.ToLower().CompareTo(playerType) == 0).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(foundPlayerType))
            {
                return PlayerType.None;
            }

            return (PlayerType) Enum.Parse(typeof(PlayerType), playerType, true);
        }
    }
}
