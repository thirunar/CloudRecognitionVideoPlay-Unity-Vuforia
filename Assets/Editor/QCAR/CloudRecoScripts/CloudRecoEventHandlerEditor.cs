/*==============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
==============================================================================*/

using UnityEditor;
using Vuforia;

/// <summary>
/// This editor class renders the custom inspector for the CloudRecoEventHandler MonoBehaviour
/// </summary>
[CustomEditor(typeof(CloudRecoEventHandler))]
public class CloudRecoEventHandlerEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    /// <summary>
    /// Draws a custom UI for the cloud reco event handler inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        CloudRecoEventHandler crehb = (CloudRecoEventHandler)target;

        EditorGUILayout.HelpBox("Here you can set the ImageTargetBehaviour from the scene that will be used to augment new cloud reco search results.", MessageType.Info);
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        crehb.ImageTargetTemplate = (ImageTargetBehaviour)EditorGUILayout.ObjectField("Image Target Template",
                                                    crehb.ImageTargetTemplate, typeof(ImageTargetBehaviour), allowSceneObjects);
    }

    #endregion // UNITY_EDITOR_METHODS
}
