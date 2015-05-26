﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GUIManager : MonoBehaviour {

	#region PRIVATE_MEMBER_VARIABLES

	private bool showCameraTips = true;

	#endregion

	#region PUBLIC_MEMBER_VARIABLES

	public GUIStyle headerStyle;
    public GUIStyle homeButtStyle;
    public GUIStyle cameraTipsStyle;
    public GUIStyle cameraTipsCloseButtStyle;

	#endregion

	// Use this for initialization
	private void OnGUI(){
        float scaleX = (float)(Screen.width)/600.0f;
        float scaleY = (float)(Screen.height)/1024.0f;

        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(scaleX, scaleY, 1));
        
        Rect boxAppHeaderRect = new Rect(0, 0, 600, 100);
        Rect homeButtRect = new Rect(20, 10, 70, 70);
        Rect cameraTipsRect = new Rect(0, 400, 500, 100);
        Rect cameraTipsCloseButtRect = new Rect(300, 380, 60, 60);

        //This is always shown on head of AR Camera.
        GUI.Box(boxAppHeaderRect, "AR Camera", headerStyle);
        if(GUI.Button(homeButtRect, "", homeButtStyle)){
			Application.LoadLevel("main_menu_before_login");
        }

        if(showCameraTips){
        	GUI.Label(cameraTipsRect, "Point the camera to the origami paper.", cameraTipsStyle);
            if(GUI.Button(cameraTipsCloseButtRect, "", cameraTipsCloseButtStyle)){
                Debug.Log("user click the close button");
                showCameraTips = false;
            }
        }

	}
}