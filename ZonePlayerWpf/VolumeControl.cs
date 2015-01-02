//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="VolumeControl"/> for handling volume of zones
    /// </summary>
    ///[DataContract]
    public sealed class VolumeControl
    {
        /// <summary>
        /// Default volume setting
        /// </summary>
        public const int DefaultVolumeValue = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeControl"/> class.
        /// </summary>
        public VolumeControl()
        {
        }

        /// <summary>
        /// Deserialize volume controls
        /// </summary>
        /// <param name="volumeSetting">Volume ocntrol settings</param>
        /// <returns>List of volume control elements</returns>
        public static List<VolumeControl> DeserializeVolumeSettings(string volumeSetting)
        {
            List<VolumeControl> volCollection = JsonConvert.DeserializeObject<List<VolumeControl>>(volumeSetting);
            return volCollection;
        }

        /// <summary>
        /// Serialize the volume control list
        /// </summary>
        /// <param name="volCollection">list of Volume ocntrol settings</param>
        /// <returns>Serialized volume control elements</returns>
        public static string SerializeVolumeSettings(List<VolumeControl> volCollection)
        {
            // Give each item a unique identifier so it is easy to update collection
            for (int inx = 0; inx < volCollection.Count; inx++)
            {
                volCollection[inx].Index = inx;
            }

            string volumeSetting = JsonConvert.SerializeObject(volCollection);
            return volumeSetting;
        }

        /// <summary>
        /// Get volume setting for playing item
        /// </summary>
        /// <param name="volCollection">Volume ocntrol settings</param>
        /// <param name="playlistItemName">Playing item name</param>
        /// <param name="playlistName">Name of the playlist</param>
        /// <returns>Index of found volume control element</returns>
        public static int GetVolumeSetting(List<VolumeControl> volCollection, string playlistName, string playlistItemName)
        {
            if (volCollection == null)
            {
                return -1;
            }

            // Get existing volume control
            VolumeControl existingControl = volCollection.Where(col => string.Compare(col.PlaylistName, playlistName) == 0 && string.Compare(col.PlaylistItemName, playlistItemName) == 0).FirstOrDefault();

            if (existingControl == null)
            {
                return -1;
            }
            else
            {
                return existingControl.Index;
            }
        }

        /// <summary>
        /// Save volume setting for channel
        /// </summary>
        /// <param name="volCollection">Volume ocntrol settings</param>
        /// <param name="playlistItemName">Playing item name</param>
        /// <param name="playlistName">Name of the playlist</param>
        /// <param name="volume"></param>
        /// <returns>List of volume control elements</returns>
        public static List<VolumeControl> SaveVolumeSetting(List<VolumeControl> volCollection, string playlistName, string playlistItemName, int volume)
        {
            if (string.IsNullOrWhiteSpace(playlistName) || string.IsNullOrWhiteSpace(playlistItemName))
            {
                // Do nothing if no playing item is specified
                return volCollection;
            }

            VolumeControl control = null;
            int index = GetVolumeSetting(volCollection, playlistName, playlistItemName);
            if (index < 0)
            {
                // New item
                control = new VolumeControl()
                {
                    Index = volCollection.Count,
                    PlaylistItemName = playlistItemName,
                    PlaylistName = playlistName,
                    Volume = DefaultVolumeValue
                };

                volCollection.Add(control);
            }
            else
            {
                control = volCollection[index];
            }

            control.Volume = volume;
            return volCollection;
        }

        /// <summary>
        /// Gets the name of the playlist
        /// </summary>
        public string PlaylistName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the item in playlist
        /// </summary>
        public string PlaylistItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the volume for the item
        /// </summary>
        public int Volume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the index for the item
        /// </summary>
        public int Index
        {
            get;
            set;
        }
    }
}
