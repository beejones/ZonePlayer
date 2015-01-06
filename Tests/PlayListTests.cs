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
    public class PlayListTests
    {
        /// <summary>
        /// Reference playlist for tests
        /// </summary>
        private const string SamplePlaylist = @".\SamplePlaylists\internet.asx";

        [TestMethod]
        public void Player_OK_LoadPlayList()
        {
            ZonePlaylist playlist = LoadPlaylist(SamplePlaylist,true, "Test", false);
            Assert.AreEqual<int>(playlist.PlayList.Count, 3);
        }

        [TestMethod]
        public void Player_OK_Play()
        {
            // Setup
            ZonePlaylist playlist = LoadPlaylist(SamplePlaylist, true, "Test", false);

            // Act
            MusicZone musicZone = new MusicZone("MyZone", PlayerType.wmp);
            musicZone.LoadPlayList(playlist.ListUri, "Test", true);
            musicZone.Play();

            // Asserts
            Assert.IsTrue(musicZone.CurrentPlayer.IsPlaying);
            musicZone.Stop();
            Assert.IsFalse(musicZone.CurrentPlayer.IsPlaying);
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
