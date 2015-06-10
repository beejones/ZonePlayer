//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------

namespace ZonePlayerInterface
{
    /// <summary>
    /// Enumeration of the supported zone player commands
    /// </summary>
    public enum Commands
    {
        /// <summary>
        /// Start playing
        /// </summary>
        Play = 1,
        /// <summary>
        /// Play default item
        /// </summary>
        PlayDefault,
        /// <summary>
        /// Stop playing
        /// </summary>
        Stop,
        /// <summary>
        /// Ring doorbel
        /// </summary>
        DoorBel,
        /// <summary>
        /// Play next item
        /// </summary>
        Next,
        /// <summary>
        /// Play first item
        /// </summary>
        First,
        /// <summary>
        /// Increase volume
        /// </summary>
        VolUp,
        /// <summary>
        /// Decrease volume
        /// </summary>
        VolDown,
        /// <summary>
        /// Set volume
        /// </summary>
        VolSet,
        /// <summary>
        /// Get volume
        /// </summary>
        VolGet,
        /// <summary>
        /// Get player status
        /// </summary>
        PlayerStatus,
        /// <summary>
        /// Play resource referenced by the uri
        /// </summary>
        PlayUri,
        /// <summary>
        /// Select item to play
        /// </summary>
        SelectItemToPlay,
        /// <summary>
        /// Retrieve list of loaded playlists
        /// </summary>
        GetPlaylists,
        /// <summary>
        /// Retrieve list of items in playlist
        /// </summary>
        GetPlaylistItems,
        /// <summary>
        /// Basic interface test
        /// </summary>
        TestInterface,
        /// <summary>
        /// Test which will also test the player logic
        /// </summary>
        TestPlayer,
        /// <summary>
        /// Command not available
        /// </summary>
        None
    }
}
