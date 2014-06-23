using UnityEngine;
using System.Collections;

public class GUICamera : MonoBehaviour 
{
	CameraController controller;
	
	void Start()
	{
		controller = GetComponent<CameraController>();
	}

	void OnGUI() 
	{
		GUI.Box(new Rect(10, 10, 140, 120), "Camera Position");

		if(GUI.Button(new Rect(20, 40, 120, 20), "3d Person")) 
		{
			controller.currentPosition = controller.cameraPositions[0];
		}

		if(GUI.Button(new Rect(20, 70, 120, 20), "Top Down")) 
		{
			controller.currentPosition = controller.cameraPositions[1];
		}
		
		if(GUI.Button(new Rect(20, 100, 120, 20), "Top Down 2")) 
		{
			controller.currentPosition = controller.cameraPositions[2];
		}
	}
}
