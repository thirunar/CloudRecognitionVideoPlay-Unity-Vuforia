/*==============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using Vuforia;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES

    // ObjectTracker reference to avoid lookups
    private ObjectTracker mObjectTracker;
//    private ContentManager mContentManager;

    // the parent gameobject of the referenced ImageTargetTemplate - reused for all target search results
    private GameObject mParentOfImageTargetTemplate;

    #endregion // PRIVATE_MEMBER_VARIABLES

	public static string mPath;

    #region EXPOSED_PUBLIC_VARIABLES

    /// <summary>
    /// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;

    #endregion

    #region ICloudRecoEventHandler_IMPLEMENTATION

    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized()
    {
        // get a reference to the Object Tracker, remember it
        mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
//        mContentManager = (ContentManager)FindObjectOfType(typeof(ContentManager));
    }

    /// <summary>
    /// visualize initialization errors
    /// </summary>
    public void OnInitError(TargetFinder.InitState initError)
    {
        switch (initError)
        {
            case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                ErrorMsg.New("Network Unavailable", "Please check your internet connection and try again.", RestartApplication);
                break;
            case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                ErrorMsg.New("Service Unavailable", "Failed to initialize app because the service is not available.");
                break;
        }
    }
    
    /// <summary>
    /// visualize update errors
    /// </summary>
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        switch (updateError)
        {
            case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
                ErrorMsg.New("Authorization Error","The cloud recognition service access keys are incorrect or have expired.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
                ErrorMsg.New("Network Unavailable","Please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
                ErrorMsg.New("Authorization Error","The cloud recognition service has been suspended.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
                ErrorMsg.New("Request Timeout","The network request has timed out, please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
                ErrorMsg.New("Service Unavailable","The service is unavailable, please try again later.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
                ErrorMsg.New("Clock Sync Error","Please update the date and time and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
                ErrorMsg.New("Unsupported Version","The application is using an unsupported version of Vuforia.");
                break;
        }
    }

    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
        if (scanning)
        {
            // clear all known trackables
            mObjectTracker.TargetFinder.ClearTrackables(false);

            // hide the ImageTargetTemplate
//            mContentManager.ShowObject(false);
        }
    }
    
    /// <summary>
    /// Handles new search results
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
        // Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
        // or to create a new ImageTargetBehaviour for each new result

        // Vuforia will return a new object with the right script automatically if you use
        // TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)
        
        //Check if the metadata isn't null
        if(targetSearchResult.MetaData == null)
        {
            return;
        }
		Debug.Log ("Meatadata: " + targetSearchResult.MetaData);
        // First clear all trackables
        mObjectTracker.TargetFinder.ClearTrackables(false);

        // enable the new result with the same ImageTargetBehaviour:
        ImageTargetBehaviour imageTargetBehaviour = mObjectTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate) as ImageTargetBehaviour;
        
        //if extended tracking was enabled from the menu, we need to start the extendedtracking on the newly found trackble.
        if(CloudRecognitionUIEventHandler.ExtendedTrackingIsEnabled)
        {
            imageTargetBehaviour.ImageTarget.StartExtendedTracking();
        }
		mPath = targetSearchResult.MetaData;
////		VIDEO HELPER NEW CODE
//		VideoPlaybackBehaviour video = ImageTargetTemplate.GetComponentInChildren<VideoPlaybackBehaviour>();
//		
//		video.m_path = @"http://192.168.1.35:8888/KFC.mp4";
//		video.VideoPlayer = new VideoPlayerHelper ();
//		video.VideoPlayer.SetFilename (video.m_path);
//		video.MediaType = VideoPlayerHelper.MediaType.ON_TEXTURE;
//	
//		video.mCurrentState = VideoPlayerHelper.MediaState.READY;
////		video.VideoPlayer.Unload();
////		video.VideoPlayer.Load (video.m_path, VideoPlayerHelper.MediaType.ON_TEXTURE, false, 0);
//		
//		Debug.Log ("Video file path: " + video.m_path + " Video player current position: " + video.VideoPlayer.GetCurrentPosition() + " video.VideoPlayer.IsPlayableOnTexture(): " + video.VideoPlayer.IsPlayableOnTexture() +" Media Type: " + video.Me);
//		
//		if (video != null && video.AutoPlay)
//		{
////			if (video.VideoPlayer.IsPlayableOnTexture())
////			{
//				VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
////				if (state == VideoPlayerHelper.MediaState.PAUSED ||
////				    state == VideoPlayerHelper.MediaState.READY ||
////				    state == VideoPlayerHelper.MediaState.STOPPED)
//				if(true)
//				{
//					// Pause other videos before playing this one
////					PauseOtherVideos(video);
//					
//					// Play this video on texture where it left off
//					video.VideoPlayer.Play(false, 0);
//				}
////				else if (state == VideoPlayerHelper.MediaState.REACHED_END)
////				{
////					// Pause other videos before playing this one
//////					PauseOtherVideos(video);
////					
////					// Play this video from the beginning
////					video.VideoPlayer.Play(false, 0);
////				}
////			}
//		}
    }

	private void PauseOtherVideos(VideoPlaybackBehaviour currentVideo)
	{
		VideoPlaybackBehaviour[] videos = (VideoPlaybackBehaviour[])
			FindObjectsOfType(typeof(VideoPlaybackBehaviour));
		
		foreach (VideoPlaybackBehaviour video in videos)
		{
			if (video != currentVideo)
			{
				if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
				{
					video.VideoPlayer.Pause();
				}
			}
		}
	}
    #endregion // ICloudRecoEventHandler_IMPLEMENTATION



    #region UNTIY_MONOBEHAVIOUR_METHODS

    /// <summary>
    /// register for events at the CloudRecoBehaviour
    /// </summary>
    void Start()
    {
        // look up the gameobject containing the ImageTargetTemplate:
        mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;

        // intialize the ErrorMsg class
        ErrorMsg.Init();

        // register this event handler at the cloud reco behaviour
        CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (cloudRecoBehaviour)
        {
            cloudRecoBehaviour.RegisterEventHandler(this);
        }
    }

    /// <summary>
    /// draw the sample GUI and error messages
    /// </summary>
    void OnGUI()
    {
        // draw error messages in case there were any
        ErrorMsg.Draw();
    }

    #endregion UNTIY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS
    
    // callback for network-not-available error message
    private void RestartApplication()
    {
        Application.LoadLevel("Vuforia-1-About");
    }
    #endregion PRIVATE_METHODS
}
