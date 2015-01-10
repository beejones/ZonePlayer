//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diagnostics;
using System.IO;
using System.Diagnostics;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="PlayertManager"/> for creating instances of <see cref="Player"/>
    /// </summary>    
    public static class PlayerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="playerType">Type of player to create</param>
        /// <param name="audioDevice">Audio device used for audio</param>
        /// <returns>New <see cref="Player"/> instance</returns>
        public static ZonePlayer Create(PlayerType playerType, WpfPanel.PanelControl handle, string audioDevice = null)
        {
            switch (playerType)
            {
                case PlayerType.axVlc:
                    return new VlcAxPlayer(audioDevice, handle);
                case PlayerType.axWmp:
                    return new WmpAxPlayer(handle);
                case PlayerType.vlc:
                    return new LibVlcPlayer(audioDevice, handle);
                case PlayerType.wmp:
                    return new WmpPlayer();
            }

            Log.Item(EventLogEntryType.Error, "Don't knwo player {0}", playerType);
            return null;
        }
    }
}
