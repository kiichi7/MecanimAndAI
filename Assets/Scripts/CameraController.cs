using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public Transform[] cameraPositions;
	public float smooth = 3f;
	public Transform currentPosition;
	
	private float smoothDelta;
	
	void Start()
	{
		currentPosition = cameraPositions[0];
	}
	
	void FixedUpdate()
	{
		smoothDelta = Time.deltaTime * smooth;
		transform.position = Vector3.Lerp(transform.position, currentPosition.position, smoothDelta);	
		transform.rotation = Quaternion.Lerp(transform.rotation, currentPosition.rotation, smoothDelta);
	}
}
