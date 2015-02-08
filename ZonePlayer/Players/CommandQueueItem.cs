//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using ZonePlayerInterface;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="CommandQueueItem"/> for creating a command queue
    /// </summary>    
    public sealed class CommandQueueItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueueItem"/> class.
        /// </summary>
        /// <param name="command">Player Command</param>
        /// <param name="item">Item to play</param>
        /// <returns>True if player is ready</returns>
        public CommandQueueItem(Commands command, ZonePlaylistItem item)
        {
            this.Command = command;
            this.Item = item;
        }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public Commands Command
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item
        /// </summary>
        public ZonePlaylistItem Item
        {
            get;
            set;
        }
    }
}
