
/*==============================================================================
            Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
 
This  Vuforia(TM) sample application in source code form ("Sample Code") for the
Vuforia Software Development Kit and/or Vuforia Extension for Unity
(collectively, the "Vuforia SDK") may in all cases only be used in conjunction
with use of the Vuforia SDK, and is subject in all respects to all of the terms
and conditions of the Vuforia SDK License Agreement, which may be found at
<a href="https://ar.qualcomm.at/legal/license" title="https://ar.qualcomm.at/legal/license">https://ar.qualcomm.at/legal/license</a>.
 
By retaining or using the Sample Code in any manner, you confirm your agreement
to all the terms and conditions of the Vuforia SDK License Agreement.  If you do
not agree to all the terms and conditions of the Vuforia SDK License Agreement,
then you may not retain or use any of the Sample Code in any manner.
==============================================================================*/

using UnityEngine;

// A custom handler that implements the ITrackableEventHandler interface.
public class TrackableEventHandlerVideos : MonoBehaviour,
ITrackableEventHandler
{
	#region PRIVATE_MEMBER_VARIABLES
	
	private TrackableBehaviour mTrackableBehaviour;
	private VideoPlaybackBehaviour video;
	
	private bool mHasBeenFound = false;
	private bool mLostTracking;
	private bool videoFinished;
	private float mSecondsSinceLost;
	private float distanceToCamera;
	
	private float mVideoCurrentPosition;
	private float mCurrentVolume;
	
	private Transform mMyModel;
	
	
	#endregion // PRIVATE_MEMBER_VARIABLES
	
	
	
	#region UNITY_MONOBEHAVIOUR_METHODS
	
	void Start()
	{
		/*for custom animations on update
Transform[] allChildren = GetComponentsInChildren<Transform>();
foreach (Transform child in allChildren) {
     // do whatever with child transform here
if (child.name == "MyModel") mMyModel = child;
}
*/
		
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
		
		video = GetComponentInChildren<VideoPlaybackBehaviour>();
		
		OnTrackingLost();
	}
	
	
	void Update()
	{
		
		if (video == null) return;
		
		//for testing audio levels while in editor 
		//distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.root.position);
		//Debug.Log(distanceToCamera);
		//Debug.Log(1.0f-(Mathf.Clamp01(distanceToCamera*0.0005f)*0.5f));
		
		//To spatialize audio: check if component is available, then on update set volume to normalized distance from tracker.
		
		if (!mLostTracking && mHasBeenFound) {
			
			/*
//whatever custom animation is performed per update frame if tracker is found
if (mMyModel)
{
mMyModel.Rotate(0.0f, -0.2666f, 0.0f);
}
*/
			//if video is playing, get distance to camera.
			if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING) {
				//Debug.Log("Video on "+ transform.root.name +" is "+ video.m_path);
				distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.root.position);
				//Debug.Log(distanceToCamera);
				mCurrentVolume = 1.0f-(Mathf.Clamp01(distanceToCamera*0.0005f)*0.5f);
				video.VideoPlayer.SetVolume(mCurrentVolume);
				
			} else if (video.CurrentState == VideoPlayerHelper.MediaState.REACHED_END) {
				
				//Loop automatically if marker is visible and video has reached the end
				//comment this out if you want the play button to appear when the video has reached the end 
				
				Debug.Log("Video Has ended, playing again");
				video.VideoPlayer.Play(false, 0);
			}
			
			
		}
		
		
		// Pause the video if tracking is lost for more than n seconds
		if (mHasBeenFound && mLostTracking && !videoFinished)
		{
			if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
			{
				//fade out volume from current if marker is lost
				//Debug.Log(mCurrentVolume - mSecondsSinceLost);
				video.VideoPlayer.SetVolume(Mathf.Clamp01(mCurrentVolume - mSecondsSinceLost));
			}
			
			//n.0f is number of seconds before playback stops when marker is lost
			if (mSecondsSinceLost > 1.0f)
			{ 
				if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
				{
					//get last position so it can resume after video is unloaded and reloaded.
					mVideoCurrentPosition = video.VideoPlayer.GetCurrentPosition();
					video.VideoPlayer.Pause();
					
					if (video.VideoPlayer.Unload())
					{
						Debug.Log ("UnLoaded Video: "+ video.m_path); 
						videoFinished = true;
					}
					
				}
			}
			
			mSecondsSinceLost += Time.deltaTime;
		}
	}
	
	#endregion // UNITY_MONOBEHAVIOUR_METHODS
	
	
	
	#region PUBLIC_METHODS
	
	// Implementation of the ITrackableEventHandler function called when the
	// tracking state changes.
	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
		    newStatus == TrackableBehaviour.Status.TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}
	
	#endregion // PUBLIC_METHODS
	
	
	
	#region PRIVATE_METHODS
	
	
	private void OnTrackingFound()
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>();
		AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();
		
		// Enable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = true;
		}
		
		// Enable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = true;
		}
		//Play audio:
		foreach (AudioSource component in audioComponents) {
			component.audio.Play();
			
		}
		
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		
		// Optionally play the video automatically when the target is found
		
		if (video != null)
		{
			
			//load Video on tracking, use local variable to skip to position left off at pause
			if (video.VideoPlayer.Load(video.m_path, VideoPlayerHelper.MediaType.ON_TEXTURE, true, mVideoCurrentPosition))
				
			{
				Debug.Log ("Loaded Video: "+ video.m_path); 
				
				// Play this video on texture where it left off
				
			}
			
			if (video.VideoPlayer.IsPlayableOnTexture())
			{
				VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
				if (state == VideoPlayerHelper.MediaState.PAUSED ||
				    state == VideoPlayerHelper.MediaState.READY ||
				    state == VideoPlayerHelper.MediaState.STOPPED)
				{
					video.VideoPlayer.Play(false, video.VideoPlayer.GetCurrentPosition());
					
				}
				else if (state == VideoPlayerHelper.MediaState.REACHED_END)
				{
					// Play this video from the beginning
					video.VideoPlayer.Play(false, 0);
				}
			}
		}
		
		mHasBeenFound = true;
		mLostTracking = false;
		
	}
	
	
	private void OnTrackingLost()
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>();
		AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();
		
		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}
		
		// Disable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = false;
		}
		
		//Pause Audio:
		foreach (AudioSource component in audioComponents) {
			component.audio.Pause();
		}
		
		
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		
		mLostTracking = true;
		mSecondsSinceLost = 0;
		videoFinished = false;
	}
	
	#endregion // PRIVATE_METHODS
}