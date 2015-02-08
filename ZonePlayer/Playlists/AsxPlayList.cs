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
using System.Xml.Linq;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlaylist"/> for reading asx items
    /// </summary>
    [DataContract]
    public sealed class AsxPlayList : ZonePlaylist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsxPlayList"/> class.
        /// </summary>
        public AsxPlayList()
        {
            this.CurrentItemIndex = 0;
            this.PlayList = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsxPlayList"/> class.
        /// </summary>
        /// <param name="list">List of playlist items</param>
        /// <param name="listName">Name of the playlist item</param>
        public AsxPlayList(List<ZonePlaylistItem> list, string listName = null)
            : this()
        {
            this.PlayList = (List<ZonePlaylistItem>)list;
            this.ListName = listName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsxPlayList"/> class.
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        public AsxPlayList(Uri listUri, string listName = null, bool randomize = false)
            : this()
        {
            this.PlayList = this.Read(listUri, listName, randomize).PlayList;
            this.ListName = listName;
            this.ListUri = listUri;
            this.Randomized = randomize;
        }
        
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
            List<ZonePlaylistItem> playList = (List<ZonePlaylistItem>)ReadPlayList(this.ListUri, randomize);
            return (ZonePlaylist)new AsxPlayList(playList, listName);
        }

        /// <summary>
        /// Read itms of playlist
        /// </summary>
        /// <param name="resource">The <see cref="Uri" to the resource/></param>
        /// <param name="randomize">True if the items in the playlist needs to be randomized</param>
        /// <returns></returns>
        private List<ZonePlaylistItem> ReadPlayList(Uri resource, bool randomize)
        {
            XDocument asx = OpenPlayList(resource);

            IEnumerable<XElement> xmlEntry =
                from record in asx.Descendants("ENTRY")
                select record;

            List<ZonePlaylistItem> list = new List<ZonePlaylistItem>();
            foreach(var entry in xmlEntry)
            {
                string title = 
                    (from titleXml in entry.Descendants("TITLE")
                    select titleXml.Value).FirstOrDefault();

                string reference =
                    (from refXml in entry.Descendants("REF")
                     select refXml.Attribute("HREF").Value).FirstOrDefault();

                string banner =
                    (from refXml in entry.Descendants("BANNER")
                     select refXml.Attribute("HREF").Value).FirstOrDefault();

                Dictionary<string, string> param =
                    (from refXml in entry.Descendants("PARAM")
                     select new KeyValuePair<string, string>(refXml.Attribute("NAME").Value, refXml.Attribute("VALUE").Value))
                     .ToDictionary(x => x.Key, x => x.Value);

                // Get optional player type from playlist
                PlayerType? playerType = null;
                string key = PlaylistManager.PlayListItemPlayerNameType.ToUpper();
                if (param.ContainsKey(key))
                {
                    playerType = PlayerTypeHelpers.GetType(param[key]);
                }

                key = PlaylistManager.PlayListItemPlayerNameType.ToLower();
                if (param.ContainsKey(key))
                {
                    playerType = PlayerTypeHelpers.GetType(param[key]);
                }

                key = PlaylistManager.PlayListItemPlayerNameType;
                if (param.ContainsKey(key))
                {
                    playerType = PlayerTypeHelpers.GetType(param[key]);
                }

                if (reference.Trim().ToLower().StartsWith("<iframe"))
                {
                    ;
                }

                Uri refTag = new Uri(PlaylistManager.AbsolutePaths(reference));
                Uri image = (string.IsNullOrEmpty(banner)) ? null : new Uri(PlaylistManager.AbsolutePaths(banner));
                AsxItem item = new AsxItem(title, refTag, PlayListType.asx, image, playerType, param);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Open asx playlist in xDocument
        /// </summary>
        /// <param name="resource">The <see cref="Uri"/> of the resource</param>
        /// <returns>Reader to the stream</returns>
        private XDocument OpenPlayList(Uri resource)
        {
            Checks.NotNull<Uri>("resource", resource);
            Log.Item(EventLogEntryType.Information, "Open item: {0}", resource.ToString());

            XDocument doc = XDocument.Load(resource.ToString(), LoadOptions.None);
            return doc;
        }
    }
}
