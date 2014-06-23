using UnityEngine;
using System.Collections;

public class Ears : Sensor
{
	
	public bool hearSteps;
	public Vector3 soundSourcePosition;

	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}
	
	void OnTriggerStay(Collider other) 
	{
        if (other.isTrigger && other.name == "StepsSound")
		{
			hearSteps = true;
			soundSourcePosition = other.transform.position;
		}
    }
	
	void OnTriggerExit(Collider other) 
	{
        if (other.isTrigger && other.name == "StepsSound")
		{
			hearSteps = false;
		}
    }
}
