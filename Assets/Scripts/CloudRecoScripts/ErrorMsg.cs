/*==============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
/// A class showing simple error messages.
/// Errors are queued and shown for a certain amount of time.
/// </summary>
public static class ErrorMsg
{
    
    #region NESTED

    /// <summary>
    /// Basic data describing an error message
    /// </summary>
    private struct ErrorData
    {
        public string Title;
        public string Text;
        public Action Callback;
    }

    /// <summary>
    /// basic data to draw an error message
    /// </summary>
    private struct ErrorDrawData
    {
        public DateTime FirstRendered;
        public string Title;
        public string Text;
        public Action Callback;
    }

    #endregion // NESTED

    private static GUIStyle mErrorTitleMessage;
    private static GUIStyle mErrorBodyMessage;
    private static GUIStyle mErrorOkButton;

    #region PRIVATE_MEMBER_VARIABLES


    // queue holding new error messages
    private static readonly Queue<ErrorData> sErrorQueue = new Queue<ErrorData>();
    // the currently shown error messages
    private static ErrorDrawData? sCurrentError = null;
    
    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    /// <summary>
    /// initilizes the class, loads UI skin and styles
    /// </summary>
    public static void Init()
    {
        mErrorTitleMessage = new GUIStyle();
        mErrorBodyMessage = new GUIStyle();
        mErrorOkButton = new GUIStyle();
        
        mErrorTitleMessage.font = Resources.Load ("SourceSansPro-Regular") as Font;
        mErrorBodyMessage.font = Resources.Load("SourceSansPro-Regular") as Font;
        mErrorOkButton.font = Resources.Load("SourceSansPro-Regular") as Font;
        
        mErrorOkButton.alignment = TextAnchor.MiddleCenter;
        mErrorTitleMessage.alignment = TextAnchor.MiddleCenter;
        mErrorBodyMessage.alignment = TextAnchor.MiddleCenter;
        
        mErrorTitleMessage.normal.background = Resources.Load("UserInterface/grayTexture") as Texture2D;
        mErrorBodyMessage.normal.background = Resources.Load("UserInterface/white_texture") as Texture2D;
        mErrorOkButton.normal.background = Resources.Load("UserInterface/capture_button_normal_XHigh") as Texture2D;
        mErrorOkButton.normal.textColor = Color.white;
        mErrorBodyMessage.wordWrap = true;
    }

    /// <summary>
    /// This adds a new error message to the queue if the same error is not currently shown
    /// </summary>
    public static void New(string errorTitle, string errorTxt)
    {
        New(errorTitle, errorTxt, null); // pass in empty callback
    }

    /// <summary>
    /// This adds a new error message to the queue if the same error is not currently shown
    /// and allows to give a callback method that will be invoked when the error dialog is closed
    /// </summary>
    public static void New(string errorTitle, string errorTxt, Action closeCallback)
    {
        // make sure to not enqueue error msgs are currently displayed:
        if (sCurrentError == null || !sCurrentError.Value.Text.Equals(errorTxt))
        {
            sErrorQueue.Enqueue(new ErrorData
                                    {
                                        Title = errorTitle,
                                        Text = errorTxt,
                                        Callback = closeCallback
                                    });
        }
    }

    /// <summary>
    /// This method draws the current error message and automatically pops new error messages 
    /// from the queue if the life time of the current error is over
    /// </summary>
    public static void Draw()
    {
        if (sCurrentError != null)
        {
            DrawPopUp ();
        }
        else if (sErrorQueue.Count > 0)
        {
            // no current error, get next from queue
            ErrorData errorData = sErrorQueue.Dequeue();
            sCurrentError = new ErrorDrawData
                                {
                                    Title = errorData.Title,
                                    Text = errorData.Text,
                                    FirstRendered = DateTime.Now,
                                    Callback = errorData.Callback
                                };
        }
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private static void DrawPopUp()
    {
        GUILayout.BeginArea(new Rect(Screen.width/2 - 150, Screen.height/2 - 120, 300, 240));

        GUI.Box(new Rect(0, 0, 300, 50), sCurrentError.Value.Title, mErrorTitleMessage);
        
        GUI.Box(new Rect(0, 50, 300, 190), string.Empty, mErrorBodyMessage);
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.Label(new Rect(1,55,300,135),sCurrentError.Value.Text,mErrorBodyMessage);
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        if(GUI.Button( new Rect(75,185,150,50),"Ok",mErrorOkButton))
        {
            CloseCurrentError();
        }
        
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// closes the current error message and invokes its callback if necessary
    /// </summary>
    private static void CloseCurrentError()
    {
        if (sCurrentError != null)
        {
            Action callback = sCurrentError.Value.Callback;
            sCurrentError = null;
            if (callback != null) callback();
        }
    }

    #endregion // PRIVATE_METHODS
}
