/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using System.Collections;
using Vuforia;

public class CloudRecoSceneManager : MonoBehaviour {
    
    #region PUBLIC_MEMBER_VARIABLES

    public ISampleAppUIEventHandler m_UIEventHandler;
    public static ViewType mActiveViewType;
    public enum ViewType {UIVIEW, ARCAMERAVIEW};
   
    #endregion PUBLIC_MEMBER_VARIABLES;
    
    #region PRIVATE_MEMBER_VARIABLES

    private SampleInitErrorHandler mPopUpMsg;
    private bool mErrorOccurred;
    private CameraDevice.FocusMode mFocusMode;

    #endregion PRIVATE_MEMBER_VARIABLES

    void Awake()
    {
        mPopUpMsg = GetComponent<SampleInitErrorHandler>();
        if (!mPopUpMsg)
        {
            mPopUpMsg = gameObject.AddComponent<SampleInitErrorHandler>();
        }

        // Check for an initialization error on start.
        QCARAbstractBehaviour qcarBehaviour = (QCARAbstractBehaviour)FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.RegisterQCARInitErrorCallback(OnQCARInitializationError);
        }
    }

    void Start () 
    {
        InputController.BackButtonTapped += OnBackButtonTapped;
        InputController.SingleTapped += OnSingleTapped;
        InputController.DoubleTapped += OnDoubleTapped;
        m_UIEventHandler.CloseView += OnTappedOnCloseButton;
        m_UIEventHandler.GoToAboutPage += OnAboutPageTapped;
        m_UIEventHandler.Bind();
        mActiveViewType = ViewType.ARCAMERAVIEW;

        if (mErrorOccurred)
        {
            mPopUpMsg.InitPopUp();
        }
    }
    
    void OnDestroy()
    {
        InputController.BackButtonTapped -= OnBackButtonTapped;
        InputController.SingleTapped -= OnSingleTapped;
        InputController.DoubleTapped -= OnDoubleTapped;
        m_UIEventHandler.CloseView -= OnTappedOnCloseButton;
        m_UIEventHandler.GoToAboutPage -= OnAboutPageTapped;
        m_UIEventHandler.UnBind();

        QCARAbstractBehaviour qcarBehaviour = (QCARAbstractBehaviour)FindObjectOfType(typeof(QCARAbstractBehaviour));
        if (qcarBehaviour)
        {
            qcarBehaviour.UnregisterQCARInitErrorCallback(OnQCARInitializationError);
        }
    }
    
    void OnGUI()
    {
        if (mErrorOccurred)
        {
            mPopUpMsg.Draw();
            return;
        }

        m_UIEventHandler.UpdateView(false);
        switch(mActiveViewType)
        {
            case ViewType.UIVIEW:
                m_UIEventHandler.UpdateView(true);
                break;
            
            case ViewType.ARCAMERAVIEW:
                break;
        }
    }
    
    void Update () 
    {
        if (mErrorOccurred)
            return;

        InputController.UpdateInput();
    }
    
    private void OnDoubleTapped()
    {
        if(mActiveViewType == ViewType.ARCAMERAVIEW)
        {
            mActiveViewType = ViewType.UIVIEW;
        }
    }
    
    private void OnTappedOnCloseButton()
    {
        mActiveViewType = ViewType.ARCAMERAVIEW;
    }
    
    private void OnBackButtonTapped()
    {
        Application.LoadLevel("Vuforia-1-About");
    }
    
    //Setting focus mode to triggerauto unsets the continuous autofocus mode. So, we invoke continuous autofocus right after.
    private void OnSingleTapped()
    {
        StartCoroutine(SetFocusModeToTriggerAuto());
    }
    
    private IEnumerator SetFocusModeToTriggerAuto()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO)) {
              mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
        }
        
        Debug.Log("Focus Mode Changed To " + mFocusMode);
        
        yield return new WaitForSeconds(1.0f);
        
        SetFocusModeToContinuousAuto();
        
    }
    
    private void SetFocusModeToContinuousAuto()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO)) {
            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
        }
        
        Debug.Log("Focus Mode Changed To " + mFocusMode);
    }
    
    private void OnAboutPageTapped()
    {
        Application.LoadLevel("Vuforia-1-About");
    }

    public void OnQCARInitializationError(QCARUnity.InitError initError)
    {
        if (initError != QCARUnity.InitError.INIT_SUCCESS)
        {
            mErrorOccurred = true;
            mPopUpMsg.SetErrorCode(initError);
        }
    }
}
