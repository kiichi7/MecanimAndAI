using UnityEngine;
using System.Collections;

public class Sign : MonoBehaviour 
{
	public LayerMask projectOnLayer;
	
	void Start() 
	{
		Detach();
	}
	
	public void AttachTo(Vector3 position)
	{
		Vector3 abovePosition = new Vector3(position.x, position.y + 1, position.z);
		RaycastHit hit;
		
		gameObject.SetActive(true);
		
		if(Physics.Raycast(abovePosition, Vector3.down, out hit, 50, projectOnLayer))
		{
			transform.position = hit.point;
		}
		else
		{
			transform.position = position;
		}
	}
	
	public void AttachTo(Transform parentTransform)
	{
		gameObject.SetActive(true);
		transform.parent = parentTransform;
		transform.localPosition = Vector3.zero;
	}
	
	public Quaternion Rotation 
	{
		get
		{
			return transform.rotation;
		}
		set
		{
			transform.rotation = value;
		}
	}
	
	public void Detach()
	{
		gameObject.SetActive(false);
	}
}
