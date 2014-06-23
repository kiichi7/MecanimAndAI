using UnityEngine;
using System.Collections;

public class LaserEyes : MonoBehaviour
{
	public LineRenderer laserPrefab;
	public float shootTime = 1f;
	
	private GuardBehaviour guardBehaviour;
	private Transform EyeL;
	private Transform EyeR;
	private LineRenderer laserL;
	private LineRenderer laserR;
	private bool shot;
	private float shootTimeCounter;
	
	void Start()
	{		
		laserL = new LineRenderer();
		laserR = new LineRenderer();

		EyeL = transform.Find("EyeL");
		EyeR = transform.Find("EyeR");
		
		guardBehaviour = transform.root.GetComponent<GuardBehaviour>(); 
	}
	
	
	void Update ()
	{
		if (!guardBehaviour.shootTarget) return;
		
		float distanceToTarget = Vector3.Distance(transform.position, guardBehaviour.shootTarget.position);
		
		if(guardBehaviour.lookWeight >= 0.9f && distanceToTarget <= guardBehaviour.shootDistance && !shot)
		{
			laserL = Instantiate(laserPrefab) as LineRenderer;
			laserR = Instantiate(laserPrefab) as LineRenderer;
			
			shot = true;
			shootTimeCounter = shootTime;
		}
		
		if (shot)
		{
			if (shootTimeCounter > 0)
			{
				shootTimeCounter -= Time.deltaTime;
			}
			else
			{
				Destroy(laserL);
				Destroy(laserR);
				
				shot = false;
			}
		}

		if(laserL != null && laserR != null)
		{
			laserL.SetPosition(0, EyeL.position);
			laserL.SetPosition(1, guardBehaviour.shootTarget.position);
			laserR.SetPosition(0, EyeR.position);
			laserR.SetPosition(1, guardBehaviour.shootTarget.position);
		}
	}
}
