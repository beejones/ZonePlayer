ZonePlayer
==========

ZonePlayer - Multiroom audio player

ZonePlayer is developed in c# visual studio 2013.
ZonePlayer is a poor man's version of a multiroom audio system.
ZonePlayer has different audio/video players. The current application ZonePlayerWpf is setup to have three players.
Each of the players can be lined to different audio channels e.g. USB devices. One can connect speakers to each audio channel 
and play or stream content in parallel to the different audio channels.

ZonePlayer has the ability to use different player technologies. ZonePlayer can dynamically switch to the right player when a player is explicit defined in the playlist. Asx playlists support this feature.
Exampple:
  <ENTRY>
      <TITLE>backgound</TITLE>
      <REF HREF = "http://youtu.be/l9_KDzS__dc"/>
	   <PARAM NAME="Player" VALUE="vlc" />
   </ENTRY>

ZonePlayer allows you to select your prefered player as default but if you want a specific player to render a specific channel this is supported.

Currently we support:
Windows Media Player: Class to render audio
Windows Media Player ActiveX: Class to render audio and video. Wmp needs to be installed
Videolan Vlc library: Class to render audio and video. Is used by default and brings the multi audio channel feature. Libvlc Is installed by default.
Videolan Vlc ActiveX: Class to render audio and video. Vlc needs to be installed and can be found here http://www.videolan.org/.

To test ZonePlayer you need to add at least one default playlist.
You can do this in: 
ZonePlayer / ZonePlayerWpf / Properties / Settings.settings 

 <Setting Name="DefaultPlaylists" Type="System.String" Scope="User"> 
 <Value Profile="(Default)">{"Internet":".\\SamplePlaylists\\internet.asx","Playlists":".\\SamplePlaylists\\internet.asx"}</Value> 
</Setting> 


ZonePlayerWpf is a sample application using WPF to illustrate how to use ZonePlayer. It allows you to select the audio channel and the player to use for rendering.
Check out my application based on ZonePlayer called AIOPlayer (All in One Player). It is designed for people who want to use their computers or AIO's to view TV (or other streaming) and control their home automation (or other browser tasks).
AIOPlayer can be found here: https://github.com/beejones/AIOPlayer

Main features:
Different audio channel that can play in parallel
Support for different player technologies (VLC and WMP)
No need to install a player. Libvlc in installed by default
Support for different playlists (m3u, asx)
Play audio from files and streams.
Play video files and streams.
Remote control by means of WCF interface.
