﻿/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour,
                                            ITrackableEventHandler
{

    #region PRIVATE_MEMBER_VARIABLES
 
    private TrackableBehaviour mTrackableBehaviour;
    private string suggestTextFromJSON;
    private string suggestText;
    private GameObject canvas;
    private GameObject GettingSuggestTxtScript;
    private string targetFoundName;
    private string modelSceneName;
    private GameObject targetObject;

    private bool showSuggestText = false;
    private bool showButtonNext = false;
    private bool showButtonBack = false;

    public GUIStyle suggestTextStyle;
    public GUIStyle buttonNextStyle;
    public GUIStyle buttonBackStyle;

    private Rect suggestTextRect = new Rect(50, 800, 500, 200);
    private Rect buttonNextRect = new Rect(500, 450, 70, 70);
    private Rect buttonBackRect = new Rect(10, 450, 70, 70);

    #endregion // PRIVATE_MEMBER_VARIABLES


    #region UNTIY_MONOBEHAVIOUR_METHODS
    
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
/*
        canvas = GameObject.Find("Canvas");
        suggestText = canvas.GetComponentInChildren<Text>();

        Debug.Log("modelscenename is  " + modelSceneName);
*/
        modelSceneName = Application.loadedLevelName;

    }


    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
            showButtonNext = true;
            showSuggestText = true;
        }
        else
        {
            OnTrackingLost();
            showButtonNext = false;
            showSuggestText = false;
        }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS


    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

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

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

        targetFoundName = mTrackableBehaviour.TrackableName;
           
        getSuggestText(modelSceneName);
        
    }

    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

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

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        showButtonNext = false;
    }

    private string getSuggestText(string modelSceneName){
        GettingStepSuggestText gettingSuggestText = new GettingStepSuggestText();

        switch(modelSceneName){
            case "cat_scene": {
                suggestTextFromJSON = gettingSuggestText.catSuggestText(targetFoundName);
                break;
            }
        }

        suggestText = suggestTextFromJSON ;
        return suggestText;
    }
	
    public void OnGUI(){
        float scaleX = (float)(Screen.width)/600.0f;
        float scaleY = (float)(Screen.height)/1024.0f;
        
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(scaleX, scaleY, 1));
        
        if(showButtonNext == true){
            if(GUI.Button(buttonNextRect, "", buttonNextStyle)){
                //targetObject = GameObject.Find(targetFoundName);
                //targetObject.SetActive(false);
            }
        }

        if(showSuggestText == true){
            suggestText = getSuggestText(modelSceneName);
            GUI.Label(suggestTextRect, suggestText, suggestTextStyle);
        }
    }

    #endregion // PRIVATE_METHODS
}
