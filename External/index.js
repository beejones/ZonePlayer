var seekBarInterval = null;
var currentIndex = 0;
var videos = [];

var DEFAULT_VIDEO_ID = 'Oi1BcouEmio';
var URL_PREFIX = 'http://gdata.youtube.com/feeds/api/';
var MAX_RESULTS = 50;
var MAX_VIDEOS = 200;
var QUERY_PARAMS = 'v=2&format=5&alt=json&fields=entry(title,media:group(media:thumbnail' +
  '[@yt:name="default"](@url),yt:videoid))&max-results=' + MAX_RESULTS;

$(document).ready(function() {
  if (window.location.hash) {
    $('#feedPath').val(window.location.hash.substring(1));
  } else {
    $('#feedPath').val('videos');
  }
  
  $('#playerDiv').hide();
  
  disable('loadFeed', 'previous', 'pause', 'play', 'next', 'volume', 'seek');
  
  var tag = document.createElement('script');
  tag.src = 'http://www.youtube.com/player_api';
  var firstScriptTag = document.getElementsByTagName('script')[0];
  firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
});

function getFeed(player, feedPath, startIndex) {
  currentIndex = 0;
  var querySeparator = '&';
  if (feedPath.indexOf('?') == -1) {
    querySeparator = '?';
  }
  
  $.ajax({
    dataType: 'jsonp',
    url: URL_PREFIX + feedPath + querySeparator + QUERY_PARAMS + '&start-index=' + startIndex,
    success: function(response) {
      if (response.feed && response.feed.entry) {
        $.each(response.feed.entry, function(index, entry) {
          videos.push({
            'title': entry['title']['$t'],
            'thumbnail': entry['media$group']['media$thumbnail'][0]['url'],
            'videoid': entry['media$group']['yt$videoid']['$t'],
          });
        });
        
        if(videos.length < MAX_VIDEOS) {
          getFeed(player, feedPath, startIndex + MAX_RESULTS);
        } else {
          playCurrentVideo(player);
        }
      } else {
        playCurrentVideo(player);
      }
    }
  });
}

function onYouTubePlayerAPIReady() {
  player = new YT.Player('player', {
    height: '390',
    width: '640',
    videoId: DEFAULT_VIDEO_ID,
    playerVars: {
      'controls': 0,
      'enablejsapi': 1,
      'html5': 1,
      'origin': window.location.host
    },
    events: {
      'onReady': onReady,
      'onStateChange': onStateChange,
      'onPlaybackQualityChange': onPlaybackQualityChange,
      'onError': onError
    }
  });
  
  $('#previousThumbnail').click(function() {
    playPreviousVideo(player);
  });
  
  $('#nextThumbnail').click(function() {
    playNextVideo(player);
  });
  
  $('#pause').click(function() {
    if ($(this).attr('disabled') == 'false') {
      player.pauseVideo();
    }
  });
  
  $('#play').click(function() {
    if ($(this).attr('disabled') == 'false') {
      player.playVideo();
    }
  });
  
  $('#volume').change(function() {
    player.setVolume($(this).val());
  });
  
  $('#seek').change(function() {
    if (seekBarInterval != null) {
      clearInterval(seekBarInterval);
    }
    
    $('#currentTime').html(secondsToMmSs($(this).val()));
  });
  
  $('#seek').mouseup(function() {
    player.seekTo($(this).val(), true);
    
    setSeekBarInterval();
  });
  
  $('#loadFeed').click(function() {
    currentIndex = 0;
    videos = [];
    
    getFeed(player, $('#feedPath').val(), 1);

    return false;
  });
}

function playPreviousVideo(player) {
  currentIndex--;
  playCurrentVideo(player);
}

function playNextVideo(player) {
  currentIndex++;
  playCurrentVideo(player);
}

function playCurrentVideo(player) {
  if (seekBarInterval != null) {
    clearInterval(seekBarInterval);
  }
  
  $('#currentTime').html(secondsToMmSs(0));
  $('#duration').html(secondsToMmSs(0));
  
  if (videos.length > 0) {
    $('#playerDiv').show();
  
    var previousVideo = videos[currentIndex - 1];
    var currentVideo = videos[currentIndex];
    var nextVideo = videos[currentIndex + 1];
  
    if (previousVideo == null) {
      $('#previousThumbnail').hide();
    } else {
      $('#previousThumbnail').show();
      $('#previousThumbnail').attr('src', previousVideo.thumbnail);
      $('#previousThumbnail').attr('title', previousVideo.title)
    }
  
    if (nextVideo == null) {
      $('#nextThumbnail').hide();
    } else {
      $('#nextThumbnail').show();
      $('#nextThumbnail').attr('src', nextVideo.thumbnail);
      $('#nextThumbnail').attr('title', nextVideo.title)
    }
  
    if (currentVideo != null) {
      $('#title').html(currentVideo.title);
      player.loadVideoById(currentVideo.videoid);
      
      enable('play');
      disable('pause');
    }
  } else {
    $('#playerDiv').hide();
  }
}

function onReady(event) {
  enable('loadFeed');
}

function onStateChange(event) {
  console.log('State is ' + event.data);
  
  var player = event.target;

  switch (event.data) {
    case YT.PlayerState.ENDED:
      if (seekBarInterval != null) {
        clearInterval(seekBarInterval);
        seekBarInterval = null;
      }
      
      var duration = Math.round(player.getDuration());
      $('#currentTime').html(secondsToMmSs(duration));
      $('#seek').val(duration);

      enable('play');
      disable('pause', 'volume', 'seek');
      
      playNextVideo(player);
    break;
      
    case YT.PlayerState.PLAYING:
      if (seekBarInterval != null) {
        clearInterval(seekBarInterval);
      }
      
      setSeekBarInterval();
      
      enable('pause', 'volume', 'seek');
      disable('play');
      
      $('#volume').val(player.getVolume());
      
      var duration = Math.round(player.getDuration());
      $('#duration').html(secondsToMmSs(duration));
      $('#seek').attr('max', duration);
    break;
      
    case YT.PlayerState.PAUSED:
      enable('play', 'volume', 'seek');
      disable('pause');
      
      if (seekBarInterval != null) {
        clearInterval(seekBarInterval);
        seekBarInterval = null;
      }
    break;
      
    case YT.PlayerState.BUFFERING:
      enable('pause', 'volume', 'seek');
      disable('play');
      
      if (seekBarInterval != null) {
        clearInterval(seekBarInterval);
        seekBarInterval = null;
      }
    break;
      
    case YT.PlayerState.CUED:
      enable('play');
      disable('pause', 'volume', 'seek');
    break;
  }
}

function enable() {
  $.each(arguments, function(i, id) {
    $('#' + id).attr('disabled', false);
  });
}

function disable() {
  $.each(arguments, function(i, id) {
    $('#' + id).attr('disabled', true);
  });
}

function secondsToMmSs(seconds) {
  var minutesValue = Math.floor(seconds / 60);
  var secondsValue = Math.floor(seconds % 60);
  if (secondsValue < 10) {
    secondsValue = '0' + secondsValue;
  }
  
  return minutesValue + ':' + secondsValue;
}

function setSeekBarInterval() {
  seekBarInterval = setInterval(function() {
    var currentTime = Math.round(player.getCurrentTime());
    $('#currentTime').html(secondsToMmSs(currentTime));
    $('#seek').val(currentTime);
  }, 1000);
}

function onPlaybackQualityChange(event) {
}

function onError(event) {
  console.log(event);
}
