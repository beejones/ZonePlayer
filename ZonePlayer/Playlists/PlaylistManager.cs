//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diagnostics;
using System.IO;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="PlaylisttManager"/> for creating instances of <see cref="ZonePlaylist"/>
    /// </summary>    
    public static class PlaylistManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylist"/> class.
        /// </summary>
        /// <param name="list">Path to the item</param>
        /// <param name="isAlwaysPlaylist">True if the item needs to be a playlist</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns></returns>
        public static ZonePlaylist Create(string list, bool isAlwaysPlaylist, string name = null, bool randomize = false)
        {
            Checks.IsNullOrWhiteSpace("list", list);
            return Create(new Uri(AbsolutePaths(list)), isAlwaysPlaylist, name, randomize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylist"/> class.
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="isAlwaysPlaylist">True if the item needs to be a playlist</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns></returns>
        public static ZonePlaylist Create(Uri listUri, bool isAlwaysPlaylist, string name = null, bool randomize = false)
        {
            Checks.NotNull<Uri>("listUri", listUri);
            if (!isAlwaysPlaylist)
            {
                // Create item as playlist
                return new ItemPlaylist(listUri, name);
            }

            switch(IsPlaylist(listUri))
            {
                case PlayListType.m3u:
                    return new M3uPlayList(listUri, name, randomize);

                case PlayListType.asx:
                    return new AsxPlayList(listUri, name, randomize);

                default:
                    throw new PlaylistNotFoundException(string.Format("Playlist {0} not recognized", listUri.ToString()));
            }
        }

        /// <summary>
        /// Convert relative paths into absolute paths for the playlists
        /// </summary>
        /// <param name="playlist">Paths to playlist</param>
        /// <returns></returns>
        public static string AbsolutePaths(string playlist)
        {
            string outPath = Directory.GetCurrentDirectory();
            string path = playlist.Trim();
            if (path.StartsWith(".\\"))
            {
                return outPath + path.Substring(1);
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// Checks type of a playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <returns>Returns type of the playlist</returns>
        public static PlayListType IsPlaylist(Uri listUri)
        {
            Checks.NotNull<Uri>("listUri", listUri);
            if (IsM3u(listUri))
            {
                return PlayListType.m3u;
            }

            if (IsAsx(listUri))
            {
                return PlayListType.asx;
            }

            return PlayListType.None;
        }

        /// <summary>
        /// Check whether file  is m3u file
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsM3u(Uri list)
        {
            // Only supported format
            return list.AbsolutePath.ToLower().EndsWith("m3u");
        }

        /// <summary>
        /// Check whether file  is asx file
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsAsx(Uri list)
        {
            // Only supported format
            return list.AbsolutePath.ToLower().EndsWith("asx");
        }
    }
}
