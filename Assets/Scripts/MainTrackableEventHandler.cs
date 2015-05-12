/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class MainTrackableEventHandler : MonoBehaviour,
                                            ITrackableEventHandler
{
    #region PRIVATE_MEMBER_VARIABLES
 
    private TrackableBehaviour mTrackableBehaviour;
    private bool isChooseModel = false;                 //this variable means user does/doesn't choose origami model to fold.
    private bool foundedTarget = false;
    private string targetFoundName;

    private string sql;
    private string modelsName;
    private string modelsSceneName;

    private Dictionary<string, string> modelDictionary;     //dictionary to keep the query from the database for origami models
    
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region PUBLIC_MEMBER_VARIABLES

    public GUIStyle windowStyle;
    public GUIStyle fontStyle;
    public GUIStyle buttonStyle;
    public GUIStyle gridviewStyle;

    //custom grid
    public int selGridInt = 0;
    public Texture[] selImage;
    private int selImgSize = 0;
    private int counter;

    #endregion

    #region UNTIY_MONOBEHAVIOUR_METHODS
    
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
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
        if(newStatus == TrackableBehaviour.Status.DETECTED || 
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            modelDictionary = new Dictionary<string, string>();
            foundedTarget = true;
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
        targetFoundName = mTrackableBehaviour.TrackableName;
        getModel(targetFoundName);

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }


    private void OnTrackingLost()
    {
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    }

    #endregion // PRIVATE_METHODS


    private void chooseModelContent(int windowID){
        float scaleX = (float)(Screen.width)/600.0f;
        float scaleY = (float)(Screen.height)/1024.0f;

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(scaleX, scaleY, 1));       
        GUILayout.BeginVertical("Box");

        if(modelDictionary.Count != 0){
            selImgSize = modelDictionary.Count;
        }

        counter = 0;
        selImage = new Texture[selImgSize];

        foreach(KeyValuePair<string, string> md in modelDictionary){
            selImage[counter] =  Resources.Load("'" + md.Key + "'") as Texture;
            counter += 1;
        }


        selGridInt = GUILayout.SelectionGrid(selGridInt, selImage, 2, gridviewStyle);
        
        if (GUILayout.Button("Start"))
            Debug.Log("You chose " + selImage[selGridInt]);
        
        GUILayout.EndVertical();        
    }

    private void OnGUI(){
        float scaleX = (float)(Screen.width)/600.0f;
        float scaleY = (float)(Screen.height)/1024.0f;

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(scaleX, scaleY, 1));
        Rect WindowRect = new Rect(30, 30, 540, 984);
        
        if(foundedTarget == true && isChooseModel == false){
            GUI.Window(1, WindowRect, chooseModelContent, "Please choose an origami model.", windowStyle);
        }
    }    

    public void getModel(string stepName){
        string connection = "URI=file:" + Application.dataPath + "/OrigamiGuruDB"; //Path to database.
        IDbConnection db_connection;

        db_connection = (IDbConnection) new SqliteConnection(connection);
        db_connection.Open();
        IDbCommand db_command = db_connection.CreateCommand();  

        sql = @"SELECT DISTINCT Models.model_name, Models.model_scene_name
                FROM Steps
                INNER JOIN Models_Steps
                ON Steps.step_id = Models_Steps.step_id
                INNER JOIN Models
                ON Models_Steps.model_id = Models.model_id
                WHERE Steps.step_name = '" + stepName + "'";

        db_command.CommandText = sql;
        IDataReader reader = db_command.ExecuteReader();
        
		while(reader.Read()){
            modelsName = reader.GetString(0);
            modelsSceneName = reader.GetString(1);

            Debug.Log("Query from database : model_name = " + modelsName + ", model_scene_name = " + modelsSceneName);

            if(modelDictionary.ContainsKey(modelsName)){
                Debug.Log("Model: " + modelsName + "is already in Dictionary.");
            }

            else{
                modelDictionary.Add(modelsName, modelsSceneName);
            }
   		}

        reader.Close();
        reader = null;
        db_command.Dispose();
        db_command = null;
        db_connection.Close();
        db_connection = null;             
    }

}
