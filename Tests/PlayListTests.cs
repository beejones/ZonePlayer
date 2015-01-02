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
        [DeploymentItem(@"TestFiles\Test1.m3u", ".")]
        public void Player_OK_LoadPlayList()
        {
            Uri path = new Uri(@"C:\Users\ronnybj\OneDrive\dev\Git\ZonePlayerWpf\Tests\bin\Debug\TestFiles\Test1.m3u", UriKind.Absolute);
            List<IPlaylistItem> items = LoadPlaylist(path, "Test", true);
            Assert.AreEqual<int>(items.Count, 5);
        }

        [TestMethod]
        [DeploymentItem(@"TestFiles\Test1.m3u", ".")]
        public void Player_OK_Play()
        {
            Uri path = new Uri(@"C:\Users\ronnybj\OneDrive\dev\Git\ZonePlayerWpf\Tests\bin\Debug\TestFiles\Test1.m3u", UriKind.Absolute);
            WmpPlayer player = new WmpPlayer();
            player.LoadPlayList(path, "Test", true);
            player.Play();
        }

        private List<IPlaylistItem> LoadPlaylist(Uri list, string name, bool randomize)
        {
            ZonePlaylist playlist = new M3uPlayList().Read(list, name, randomize);
            return playlist.PlayList;
        }

        private static string GetPath(string relativeFileName)
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return resources.FirstOrDefault(x => x.EndsWith(relativeFileName));
        }
    }
}
