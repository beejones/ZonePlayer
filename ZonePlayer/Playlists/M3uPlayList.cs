//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Diagnostics;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlaylist"/> for reading m3u items
    /// </summary>
    [DataContract]
    public sealed class M3uPlayList : ZonePlaylist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="M3uPlayList"/> class.
        /// </summary>
        public M3uPlayList()
        {
            this.CurrentItemIndex = 0;
            this.PlayList = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="M3uPlayList"/> class.
        /// </summary>
        /// <param name="list">List of playlist items</param>
        /// <param name="listName">Name of the playlist item</param>
        public M3uPlayList(List<ZonePlaylistItem> list, string listName = null)
            : this()
        {
            this.PlayList = (List<ZonePlaylistItem>)list;
            this.ListName = listName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="M3uPlayList"/> class.
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        public M3uPlayList(Uri listUri, string listName = null, bool randomize = false)
            : this()
        {
            this.PlayList = this.Read(listUri, listName, randomize).PlayList;
            this.ListName = listName;
            this.ListUri = listUri;
            this.Randomized = randomize;
        }

        /// <summary>
        /// Gets whether the playlist is randomized
        /// </summary>
        public override bool Randomized
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the playlist
        /// </summary>
        public override string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the playlist
        /// </summary>
        public override Uri ListUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="PlayerType"/> of the playlist
        /// The player type defines the component that will be used to render the item
        /// </summary>
        public override PlayerType PlayerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the playlist list
        /// </summary>
        public override List<ZonePlaylistItem> PlayList
        {
            get;
            set;
        }
        
        /*
                /// <summary>
                /// Gets the  current <see cref="ZonePlaylistItem"/> of the playlist
                /// </summary>
                public override ZonePlaylistItem CurrentItem
                {
                    get
                    {
                        return this.PlayList[this.CurrentItemIndex];
                    }
                }
        */
        /// <summary>
        /// Gets the <see cref="ZonePlaylistItem"/> to populate the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns>List of <see cref="ZonePlaylistItem"/> </returns>
        public override ZonePlaylist Read(Uri listUri, string listName, bool randomize)
        {
            this.ListUri = Checks.NotNull<Uri>("ListUri", listUri);
            this.ListName = listName;
            List<ZonePlaylistItem> playList = (List<ZonePlaylistItem>)ReadPlayList(this.ListUri, randomize);
            return (ZonePlaylist)new M3uPlayList(playList, listName);
        }

        /// <summary>
        /// Read itms of playlist
        /// </summary>
        /// <param name="resource">The <see cref="Uri" to the resource/></param>
        /// <param name="randomize">True if the items in the playlist needs to be randomized</param>
        /// <returns></returns>
        private List<ZonePlaylistItem> ReadPlayList(Uri resource, bool randomize)
        {
            using (TextReader tr = OpenPlayList(resource))
            {

                // read a line of text
                string data;
                List<ZonePlaylistItem> m3uData = new List<ZonePlaylistItem>();
                List<int> listToRandomize = new List<int>();
                int cnt = 0;
                while ((data = tr.ReadLine()) != null)
                {
                    if (data.Trim().Length == 0)
                        continue;

                    if (data.Contains("#EXT"))
                        continue;

                    data = PlaylistManager.AbsolutePaths(data);

                    m3uData.Add((ZonePlaylistItem)new M3uItem(
                            null,
                            new Uri(data),                            
                            PlayListType.m3u,
                            null));
                    listToRandomize.Add(cnt++);
                }
             
                // close the stream
                tr.Close();

                List<ZonePlaylistItem> result;
                if (randomize && listToRandomize.Count > 1)
                {
                    var shuffledcards = listToRandomize.OrderBy(a => Guid.NewGuid());
                    result = new List<ZonePlaylistItem>();
                    foreach (int i in shuffledcards)
                    {
                        result.Add(m3uData[i]);
                    }
                }
                else
                    result = m3uData;

                return (List<ZonePlaylistItem>)result;
            }
        }

        /// <summary>
        /// Open stream to read m3u playlist
        /// </summary>
        /// <param name="resource">The <see cref="Uri"/> of the resource</param>
        /// <returns>Reader to the stream</returns>
        private TextReader OpenPlayList(Uri resource)
        {
            Checks.NotNull<Uri>("resource", resource);
            Log.Item(EventLogEntryType.Information, "Open item: {0}", resource.ToString());

            // Open a connection
            using (WebClient client = new WebClient())
            {
                string list = client.DownloadString(resource);
                return new StringReader(list);
            }
        }
    }
}
