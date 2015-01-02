using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="PlaylistNotFoundException"/>
    /// </summary>    
    public class PlaylistNotFoundException : Exception
    {
        public PlaylistNotFoundException(string message) : base(message)
        {
        }
    }
}
