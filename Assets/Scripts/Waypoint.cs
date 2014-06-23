using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour 
{
	public int id;
	
	void Start() 
	{
	
	}

	void Update() 
	{
	
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}
}
