using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FOV2DEyes : MonoBehaviour
{
	public bool raysGizmosEnabled = false;
	public float castRate = 0.05f;
	public int quality = 2;
	public int fovAngle = 90;
	public float fovMaxDistance = 15;
	public LayerMask ignoringLayers;
	public List<RaycastHit> hits = new List<RaycastHit>();
	
	private int numRays;
	private float currentAngle;
	private Vector3 direction;
	private RaycastHit hit;
	
	void Start() 
	{
		InvokeRepeating("CastRays", 0, castRate);
	}
	
	void CastRays()
	{
		numRays = fovAngle * quality;
		currentAngle = fovAngle / -2;
		
		hits.Clear();
		
		for (int i = 0; i < numRays; i++)
		{
			direction = Quaternion.AngleAxis(currentAngle, transform.up) * transform.forward;
			hit = new RaycastHit();
			
			if(Physics.Raycast(transform.position, direction, out hit, fovMaxDistance, ~ignoringLayers) == false)
			{
				hit.point = transform.position + (direction * fovMaxDistance);
			}
			
			hits.Add(hit);

			currentAngle += 1f / quality;
		}
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		
		if (raysGizmosEnabled && hits.Count() > 0) 
		{
			foreach (RaycastHit hit in hits)
			{
				Gizmos.DrawSphere(hit.point, 0.04f);
				Gizmos.DrawLine(transform.position, hit.point);
			}
		}
	}
	
}
