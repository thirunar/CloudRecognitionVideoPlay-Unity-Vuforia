/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * Confidential and Proprietary – Qualcomm Connected Experiences, Inc. 
 * ==============================================================================*/


using UnityEngine;
using System.Collections;

public class AboutManager : MonoBehaviour {
    
    #region PRIVATE_MEMBER_VARIABLES
    public string TitleForAboutPage = "About";
    private AboutScreenView mAboutView;
    private InputController mInputController;
    
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region UNITY_MONOBEHAVIOUR_METHODS
    void Start () {
        
        mAboutView = new AboutScreenView();
        mAboutView.SetTitle(TitleForAboutPage);
        mAboutView.OnStartButtonTapped += OnAboutStartButtonTapped;
        mAboutView.LoadView();
    }
    
    void Update () {
    
        //Android devices' back-button-press is same as pressing escape
        //Exit the app when user presses the back button
        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    
    void OnGUI()
    {
        mAboutView.UpdateUI(true);
    }
    #endregion UNITY_MONOBEHAVIOUR_METHODS
    
    private void OnAboutStartButtonTapped()
    {
        Application.LoadLevel("Vuforia-2-LoadingScene");
    }
    
}
