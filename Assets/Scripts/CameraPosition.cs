using UnityEngine;
using System.Collections;

public class CameraPosition : MonoBehaviour 
{
	public bool fixInitialRotation;
	public bool fixInitialPosition;
	public Transform lookAt;
	
	private Quaternion initialRotation;
	private Vector3 initialPosition;

	void Start () 
	{
		initialRotation = transform.rotation;
		initialPosition = transform.position;
	}
	
	void Update () 
	{
		if (fixInitialRotation)
		{
			transform.rotation = initialRotation;
		}
		
		if (fixInitialPosition)
		{
			transform.position = initialPosition;
		}
		
		if (lookAt != null)
		{
			transform.LookAt(lookAt);
		}
	}
}
