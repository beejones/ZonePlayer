ZonePlayer
==========

ZonePlayer - Multiroom audio player

ZonePlayer is developed in c# visual studio 2013.
ZonePlayer is a poor man's version of a multiroom audio system.
ZonePlayer has different audio/video players. The current application ZonePlayerWpf is setup to have three players.
Each of the players can be lined to different audio channels e.g. USB devices. One can connect speakers to each audio channel 
and play or stream content in parallel to the different audio channels.

To test ZonePlayer you need to add at least one default playlist.
You can do this in: 
ZonePlayer / ZonePlayerWpf / Properties / Settings.settings 

 <Setting Name="DefaultPlaylists" Type="System.String" Scope="User"> 
 <Value Profile="(Default)">{"Internet":".\\SamplePlaylists\\internet.asx","Playlists":".\\SamplePlaylists\\internet.asx"}</Value> 
</Setting> 


ZonePlayerWpf is a sample application using WPF to illustrate how to use ZonePlayer. 
Check out my application based on ZonePlayer called AIOPlayer (All in One Player). It is designed for people who want to use their computers or AIO's to view TV (or other streaming) and control their home automation (or other browser tasks).
AIOPlayer can be found here: https://github.com/beejones/AIOPlayer

Main features:
Different audio channel that can play in parallel
Support for different playlists (m3u, asx)
Play audio from files and streams.
Play video files and streams.
Remote control by means of WCF interface.
