//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="Playlist"/> for modelling a playlist
    /// </summary>    
    public sealed class Playlist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Playlist"/> class.
        /// </summary>
        /// <param name="name">Set name for the playlist/></param>
        /// <param name="uri">Set uri for the playlist/></param>
        public Playlist(string name, Uri uri)
        {
            this.Name = name;
            this.Uri = uri;
        }

        /// <summary>
        /// Gets the name of the playlist
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the playlist
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return string.Format("Name: '{0}', Uri: '{1}'", this.Name, this.Uri.ToString());
        }
    }
}
