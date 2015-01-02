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

11     <Setting Name="DefaultPlaylists" Type="System.String" Scope="User"> 
12       <Value Profile="(Default)">{"Satellite":"C:\\Users\\ronnybj\\OneDrive\\Playlists\\satellite.asx","Internet":"C:\\Users\\ronnybj\\OneDrive\\Playlists\\internet.asx","Playlists":"C:\\Users\\ronnybj\\OneDrive\\Playlists\\playlists.asx"}</Value> 
13     </Setting> 

