//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZonePlayer;

namespace Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Switch_Player_based_on_playlist_OK_Play()
        {
            // Setup
            MusicZone musicZone = new MusicZone("MyZone");
            musicZone.LoadPlayList(new Uri(PlaylistManager.AbsolutePaths(TestReferences.SamplePlaylist1)), "Test", true);
            ZonePlaylist items = musicZone.CurrentPlaylist;
            List<string> allPlayers = PlayerTypeHelper.GetPlayerTypes();
            
            // Add each player type to the playlist and try to play it.
            // Test whether the playlist type is being used as player
            foreach(var p in allPlayers)
            {
                if (p == "None")
                {
                    continue;
                }

                PlayerType playerType = PlayerType.None;
 
                // Set play type in playlist
                foreach(var i in items.PlayList)
                {
                    playerType =  PlayerTypeHelper.GetType(p);
                    i.PlayerType = playerType;
                }

                musicZone.Play();
                Assert.IsTrue(musicZone.CurrentPlayer.PlayerType == playerType);
                //Assert.IsTrue(musicZone.CurrentPlayer.IsPlaying);
                musicZone.Stop();
            }
        }

        /// <summary>
        /// Load reference playlists
        /// </summary>
        /// <param name="list">Uri to playlist</param>
        /// <param name="isAlwaysPlaylist">True if playlist can only be a playlist and not a single item</param>
        /// <param name="name">Name of playlist</param>
        /// <param name="randomize">True if playlist needs to be randomized</param>
        /// <returns></returns>
        private ZonePlaylist LoadPlaylist(string list, bool isAlwaysPlaylist, string name, bool randomize)
        {
            ZonePlaylist playlist = PlaylistManager.Create(list, isAlwaysPlaylist, name, randomize);
            return playlist;
        }
    }
}
