//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using Declarations.Media;
using Declarations.Players;
using Diagnostics;
using Implementation;
using ZonePlayerInterface;
using System.Threading;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="YoutubePlayer"/> for rendering media by means of a browser.
    /// </summary>
    public sealed class YoutubePlayer : ZonePlayer
    {
        /// <summary>
        /// Constant defining the player file
        /// </summary>
        private const string PlayerFile = "YoutubePlayer_1a.html";

        /// <summary>
        /// Constant defining play command
        /// </summary>
        private const string CmdPlay = "window.start('{0}');";

        /// <summary>
        /// Constant defining test command
        /// </summary>
        private const string CmdTest = "window;";

        /// <summary>
        /// Constant defining stop command
        /// </summary>
        private const string CmdStop = "window.stop();";

        /// <summary>
        /// Constant defining isPlaying command
        /// </summary>
        private const string CmdIsPlaying = "window.isPlaying();";

        /// <summary>
        /// Constant to define main youtube host
        /// </summary>
        private const string HostMainYoutube = "youtube.com";

        /// <summary>
        /// Constant to define youtube share host
        /// </summary>
        private const string HostShareYoutube = "youtu.be";

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubePlayer"/> class.
        /// </summary>
        /// <param name="audioDevice">Audio device for the player</param>
        /// <param name="handle">Handle to rendering panel</param>
        public YoutubePlayer(string audioDevice, WpfPanel.PanelControl handle)
        {
            Log.Item(EventLogEntryType.Information, "Initialize YoutubePlayer player for device: {0}", audioDevice);
            SetNativePlayer(handle);
        }

        /// <summary>
        /// Dispose webBrowser
        /// </summary>
        ~YoutubePlayer()
        {
            this.NativePlayer.Dispose();
        }

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public override PlayerType PlayerType
        {
            get
            {
                return PlayerType.youtube;
            }
        }

        /// <summary>
        /// Gets or sets the audio volume for the player
        /// </summary>
        public override int Volume
        {
            get
            {
                return this.CurrentVolume;
            }
            set
            {
                Log.Item(EventLogEntryType.Information, "Set volume YoutubePlayer player: {0}", value);
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                bool ready = this.ReadyForJavascript();
                if (!ready)
                {
                    return false;
                }

                JSValue result = this.JavascriptCommand(CmdIsPlaying, null);
                if (result == null)
                {
                    return false;
                }

                int playerStatus = (int)result;
                return playerStatus == 1 || playerStatus == 3;
            }
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public override void Play()
        {
            Checks.NotNull<ZonePlaylist>("CurrentPlayList", this.CurrentPlayList);
            ZonePlaylistItem item = null;

            if (this.CurrentPlayList.CurrentItem == null)
            {
                item = this.CurrentPlayList.PlayList.First();
            }

            item = Checks.NotNull<ZonePlaylistItem>("CurrentItem", this.CurrentPlayList.CurrentItem);
            this.PlayItem(item);
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public override void Stop()
        {
            Log.Item(EventLogEntryType.Information, "Stop YoutubePlayer");

            bool ready = this.PlayerReadyForCommand(Commands.Stop, null);
            if (!ready)
            {
                Log.Item(EventLogEntryType.Warning, "Youtube player not ready. Can't stop item: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
                return;
            }

            bool isPlaying = this.NativePlayer.ExecuteJavascriptWithResult(CmdIsPlaying);
            bool result = this.NativePlayer.ExecuteJavascriptWithResult(CmdStop);
        }

        /// <summary>
        /// Check whether player can render item
        /// </summary>
        /// <param name="item">Item to test</param>
        /// <returns>True if player can render item</returns>
        public override bool CanPlayItem(Uri item)
        {
            return GetYouTubeVideoId(item) != null;
        }

        /// <summary>
        /// Gets the native implemention of the player
        /// </summary>
        private WebControl NativePlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the reference to the player
        /// </summary>
        private string NativePlayerReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Indicate main frame is loaded
        /// </summary>
        private bool MainFrameLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application timer
        /// </summary>
        private Timer PollingTimer
        {
            get;
            set;
        }

        /// <summary>
        /// Queue to cache commands
        /// </summary>
        private List<CommandQueueItem> Queue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the windows handle
        /// </summary>
        private WpfPanel.PanelControl Handle
        {
            get;
            set;
        }



        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        private void PlayItem(ZonePlaylistItem item)
        {
            Checks.NotNull<ZonePlaylistItem>("item", item);
            Log.Item(EventLogEntryType.Information, "Play: {0}", this.CurrentPlayList.CurrentItem.ItemUri);

            // Is this a youtbue item?
            if (!this.CanPlayItem(item.ItemUri))
            {
                throw new CanNotPlayItemException("Not a youtube video", item.ItemUri);
            }

            // Get video id. Skip leading /
            string videoId = this.GetYouTubeVideoId(item.ItemUri);

            bool ready = this.PlayerReadyForCommand(Commands.Play, item);
            if (!ready)
            {
                Log.Item(EventLogEntryType.Warning, "Youtube player not ready. Can't play item: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
                return;
            }

            bool isPlaying = this.IsPlaying;
            string cmd = string.Format(CmdPlay, videoId);
            JSValue result = this.JavascriptCommand(cmd, new CommandQueueItem(Commands.Play, item));
        }

        /// <summary>
        /// Return the video id of the youtube video
        /// </summary>
        /// <param name="item">vido to render</param>
        /// <returns>video id of youtbue video. Null if utem is not a youtube video</returns>
        private string GetYouTubeVideoId(Uri item)
        {
            string videoId = null;
            if (item.ToString().Contains(HostMainYoutube))
            {
                string prefix = "/watch?v=";
                videoId = item.PathAndQuery.Substring(prefix.Length);
            }
            else
            {
                if (item.ToString().Contains(HostShareYoutube))
                {
                    videoId = item.PathAndQuery.Substring(1);
                }
            }


            // Get video id. Skip leading /
            return videoId;
        }

        /// <summary>
        /// Add item to the queue
        /// </summary>
        /// <param name="command">Player Command</param>
        /// <param name="item">Item to play</param>
        private void AddToQueue(Commands command, ZonePlaylistItem item)
        {
            CommandQueueItem queueItem = new CommandQueueItem(command, item);
            Queue.Add(queueItem);
        }

        /// <summary>
        /// Get item from the queue
        /// </summary>
        private CommandQueueItem GetItemFromQueue()
        {
            CommandQueueItem queueItem = null;

            if (Queue.Count > 0)
            {
                queueItem = Queue[0];
                Queue.RemoveAt(0);
            }

            return queueItem;
        }

        /// <summary>
        /// Check whether youtube player is reader to receive commands
        /// If not delay execution of command until ready.
        /// </summary>
        /// <param name="command">Player Command</param>
        /// <param name="item">Item to play</param>
        /// <returns>True if player is ready</returns>
        private bool PlayerReadyForCommand(Commands command, ZonePlaylistItem item)
        {
            bool ready = this.ReadyForJavascript();
            if (!ready || this.JavascriptCommand(CmdTest, null) == null)
            {
                Log.Item(EventLogEntryType.Warning, "Youtube player not ready");
                AddToQueue(command, item);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Is the document ready to receive javascript commands
        /// </summary>
        /// <returns></returns>
        private bool ReadyForJavascript()
        {
            bool ready = this.NativePlayer.IsLive && this.NativePlayer.IsLoaded && this.NativePlayer.IsDocumentReady && !this.NativePlayer.IsNavigating && this.MainFrameLoaded;
            return ready;
        }

        /// <summary>
        /// Execute javascript player command
        /// </summary>
        /// <param name="command">player command</param>
        /// <returns>Value from javascript funcrion</returns>
        private JSValue JavascriptCommand(string command, CommandQueueItem item)
        {
            if (this.ReadyForJavascript())
            {
                JSValue jsResult = this.NativePlayer.ExecuteJavascriptWithResult(command);
                Error lastError = NativePlayer.GetLastError();
                bool errorOccured = true;
                switch (lastError)
                {
                    case Error.None:
                        Log.Item(EventLogEntryType.Information, "{0} executed", command);
                        errorOccured = false;
                        break;
                    case Error.TimedOut:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was TimedOut");
                        break;
                    case Error.ConnectionGone:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was ConnectionGone");
                        break;
                    case Error.WebViewGone:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was WebViewGone");
                        break;
                    case Error.ObjectGone:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was ObjectGone, that is object no longer exiet?");
                        break;
                    case Error.BadParameters:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was BadParameters?");
                        break;
                    case Error.Generic:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was Generic, we don't know what happened");
                        break;
                    default:
                        Log.Item(EventLogEntryType.Error, "Cause of failure was unknown error code");
                        break;
                }

                if (errorOccured)
                {
                    if (command != CmdTest && item != null)
                    {
                        // Add command back into queue
                        Queue.Insert(0, item);
                    }

                    Log.Item(EventLogEntryType.Error, "Failed command: {0}", command);
                    return null;
                }
                else
                {
                    Log.Item(EventLogEntryType.Information, "Success command: {0}", command);
                    return jsResult;
                }
            }
            else
            {
                Log.Item(EventLogEntryType.Warning, "Not ready to send javascript commands");
            }

            return null;
        }

        /// <summary>
        /// Polling for command while player is in setup mode.
        /// </summary>
        /// <param name="o"></param>
        private void TimerPoll(Object o)
        {
            if (Queue.Count <= 0)
            {
                return;
            }

            this.NativePlayer.Dispatcher.BeginInvoke((Action)(() => 
                {
                    if (this.ReadyForJavascript() && this.MainFrameLoaded && this.JavascriptCommand(CmdTest, null) != null)
                    {
                        CommandQueueItem queueItem = this.GetItemFromQueue();

                        Log.Item(EventLogEntryType.Information, "DocumentReady: ready to process javascript: {0}", queueItem.Command);
                            switch (queueItem.Command)
                            {
                                case Commands.Play:
                                    this.PlayItem(queueItem.Item);
                                    break;
                                case Commands.Stop:
                                    this.Stop();
                                    break;
                                default:
                                    throw new NotImplementedException("Command not supported in youtube player");
                            }
                        }
                }));
        }


        /// <summary>
        /// Try to create uri
        /// </summary>
        /// <param name="url">Input url</param>
        /// <param name="uri">Resulting <see cref="Uri"/></param>
        /// <returns>True if it was possible to create the Uri</returns>
        private bool TryCreateUri(string url, out Uri uri)
        {
            uri = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadString(url);
                    uri = new Uri(url);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Set handle to panel if not yet initialized
        /// </summary>
        /// <param name="handle">Handle to rendering panel</param>
        private void SetNativePlayer(WpfPanel.PanelControl handle)
        {
            this.MainFrameLoaded = false;
            this.Handle = handle;
            this.Queue = new List<CommandQueueItem>();
            this.NativePlayer = new WebControl();
            this.Handle.Content = this.NativePlayer;

            // Find html player
            string url = string.Format("file://{0}/{1}", Directory.GetCurrentDirectory(), PlayerFile);
            Uri toPlay;
            if (this.TryCreateUri(url, out toPlay))
            {
                this.NativePlayerReference = toPlay.ToString();
            }
            else
            {
                url = string.Format("file://{0}/Players/Html/{1}", Directory.GetCurrentDirectory(), PlayerFile);
                if (this.TryCreateUri(url, out toPlay))
                {
                    this.NativePlayerReference = toPlay.ToString();
                }
            }

            this.NativePlayer.LoadingFrameComplete += (s, e) =>
            {
                if (this.MainFrameLoaded)
                {
                    return;
                }

                this.MainFrameLoaded = e.IsMainFrame;
                if (this.MainFrameLoaded)
                {
                    Log.Item(EventLogEntryType.Information, "LoadingFrameComplete: Main frame loaded");
                }
            };

            // Setup the player
            this.NativePlayer.DocumentReady += (s, e) =>
                {
                    Log.Item(EventLogEntryType.Information, "DocumentReady : {0}", this.ReadyForJavascript());
                };

            this.NativePlayer.Source = new Uri(this.NativePlayerReference);
            this.PollingTimer = new Timer(TimerPoll, null, 500, 500);
        }
    }
}