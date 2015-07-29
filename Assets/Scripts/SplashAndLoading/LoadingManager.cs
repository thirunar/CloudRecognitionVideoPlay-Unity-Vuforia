/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * Confidential and Proprietary – Qualcomm Connected Experiences, Inc. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;

/// <summary>
/// Loading manager.
/// 
/// This Script handles the loading of the Main scene in background
/// displaying a loading animation while the scene is being loaded
/// </summary>
public class LoadingManager : MonoBehaviour
{

    #region PRIVATE_MEMBER_VARIABLES
     private Texture Spinner;
    private bool mChangeLevel = true;
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS
    void Awake()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Spinner = Resources.Load("UserInterface/spinner_XHigh") as Texture;
    }

    void Start()
    {
        Resources.UnloadUnusedAssets();

        System.GC.Collect();

        Application.backgroundLoadingPriority = ThreadPriority.Low;

        mChangeLevel = true;
    }

    void Update()
    {
        if (mChangeLevel)
        {
            LoadUserDefTargetsScene();
            mChangeLevel = false;
        }
    }


    void OnGUI()
    {
        Matrix4x4 oldMatrix = GUI.matrix;
        float thisAngle = Time.frameCount*4;

        Rect thisRect = new Rect(Screen.width/2.0f - Spinner.width/2f, Screen.height/2.0f - Spinner.height/2f,
                                 Spinner.width, Spinner.height);

        GUIUtility.RotateAroundPivot(thisAngle, thisRect.center);
        GUI.DrawTexture(thisRect, Spinner);
        GUI.matrix = oldMatrix;
    }
    #endregion UNITY_MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    private void LoadUserDefTargetsScene()
    {
         Application.LoadLevelAsync("Vuforia-3-CloudRecognition");
    }

    #endregion PRIVATE_METHODS
}
