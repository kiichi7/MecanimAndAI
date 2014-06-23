using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour 
{
	public bool loop;
	public List<Waypoint> points;
	
	private Waypoint targetPoint;
	
	void Start() 
	{
		Waypoint[] waypoints = GetComponentsInChildren<Waypoint>();
		
		foreach(Waypoint waypoint in waypoints)
		{
			points.Add(waypoint);
		}
		
		points.Sort(delegate(Waypoint point1, Waypoint point2) {
			return point1.id.CompareTo(point2.id);
		});
		
		targetPoint = points[0];
	}
	
	void Update() 
	{
	
	}
	
	public Waypoint TargetPoint
	{
		get
		{
			return targetPoint;
		}
	}
	
	public void SetNextPoint()
	{
		if (points.IndexOf(targetPoint) == points.Count - 1)
		{
			if (loop)
			{
				targetPoint = points[0];
			}
		}
		else
		{
			targetPoint = points[points.IndexOf(targetPoint) + 1];
		}
	}
	
}
