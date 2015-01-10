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

        [TestMethod]
        public void Player_OK_LoadPlayList()
        {
            ZonePlaylist playlist1 = LoadPlaylist(TestReferences.SamplePlaylist1, true, "Test1", true);
            ZonePlaylist playlist2 = LoadPlaylist(TestReferences.SamplePlaylist2, true, "Test2", true);
            Assert.AreEqual<int>(playlist1.PlayList.Count, TestReferences.SamplePlaylistCount1);
            Assert.AreEqual<int>(playlist2.PlayList.Count, TestReferences.SamplePlaylistCount2);
        }

        [TestMethod]
        public void Player_OK_Play()
        {
            // Setup
            ZonePlaylist playlist = LoadPlaylist(TestReferences.SamplePlaylist1, true, "Test", false);

            // Act
            MusicZone musicZone = new MusicZone("MyZone", PlayerType.wmp, null, null);
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
