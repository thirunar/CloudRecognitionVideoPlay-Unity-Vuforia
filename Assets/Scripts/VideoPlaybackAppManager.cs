/*============================================================================== 
 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class VideoPlaybackAppManager : AppManager {
    
    public override void InitManager ()
    {
        base.InitManager ();
        InputController.DoubleTapped += HandleDoubleTap;
    }

    public override void DeInitManager()
    {
        base.DeInitManager();
        
        InputController.DoubleTapped -= HandleDoubleTap;
    }
    
    public override void UpdateManager ()
    {
        base.UpdateManager ();
    }
    
    #region PRIVATE_METHODS

    /// <summary>
    /// Handle double tap event
    /// </summary>
    private void HandleDoubleTap()
    {
        // Get currently playing video, if any,
        // and pause it before the UI menu is opened.
        // This is needed in Unity 5 in order to show the UI menu
        VideoPlaybackBehaviour video = GetPlayingVideo();
        if (video != null && video.VideoPlayer.IsPlayableOnTexture()) {
            video.VideoPlayer.Pause();
        }
    }
    
    /// <summary>
    /// Returns the currently active (playing) video, if any
    /// </summary>
    private VideoPlaybackBehaviour GetPlayingVideo()
    {
        VideoPlaybackBehaviour[] videos = (VideoPlaybackBehaviour[])
                FindObjectsOfType(typeof(VideoPlaybackBehaviour));
        
        foreach (VideoPlaybackBehaviour video in videos)
        {
            if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
            {
                return video;
            }
        }
        return null;
    }
    
    #endregion // PRIVATE_METHODS
    
}
