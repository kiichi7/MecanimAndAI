using UnityEngine;
using System.Collections;

public class GuardBehaviour : MonoBehaviour
{
	
	public enum Status
	{
		Idle,
		Following,
		Searching
	}

	private Transform targetTransform;
	private Vector3 targetPosition;
	private NavMeshAgent agent;
	private Animator animator;
	private Locomotion locomotion;
	private float searchTimeCurrent;
	private float searchTimeCounter;
	private Vector3 centerOfSearchArea;
	private float idleAfterShootingTimeCounter;
	
	public Status status;
	public Sign alertSign;
	public WaypointManager waypointManager;
	public float searchTime;
	public bool sawOrHeardTargetPerson;
	public float searchRadius;
	public float runSpeed = 3.5f;
	public float walkSpeed = 1f;
	public float shootDistance = 3f;
	public float idleTimeAfterShooting = 10f;
	[System.NonSerialized]
	public Transform shootTarget;
	[System.NonSerialized]					
	public float lookWeight;
	[System.NonSerialized]
	public Transform targetTransformInSight;
	public FOV2DEyes eyes;
	public Ears ears;
	public FOV2DVisionCone visionCone;

	void Start ()
	{
		targetPosition = transform.position;
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		searchTimeCurrent = 0;
	}
	
	void Update() 
	{
		SetupAgentLocomotion();
		StatusUpdate();
	}
	
	void FixedUpdate()
	{
		if (idleAfterShootingTimeCounter > 0)
		{
			idleAfterShootingTimeCounter -= Time.fixedDeltaTime;
		}
	}
	
	void SetupAgentLocomotion()
	{
		if (AgentDone())
		{
			locomotion.Do(0, 0);
		}
		else
		{
			float speed = agent.desiredVelocity.magnitude;

			Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;

			float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;

			locomotion.Do(speed, angle);
		}
	}

    void OnAnimatorMove()
    {
        agent.velocity = animator.deltaPosition / Time.deltaTime;
		transform.rotation = animator.rootRotation;
    }

	bool AgentDone()
	{
		return !agent.pathPending && AgentStopping();
	}

	bool AgentStopping()
	{
		return agent.remainingDistance <= agent.stoppingDistance;
	}
	
	void StatusUpdate()
	{
		int hitsInPerson = 0;
		targetTransformInSight = null;

		foreach (RaycastHit hit in eyes.hits) 
		{
			if (hit.transform && hit.transform.tag == "Player") 
			{
				hitsInPerson++;
				targetTransformInSight = hit.transform;
				shootTarget = targetTransformInSight.FindChild("ShootTarget");
				if (!shootTarget)
				{
					shootTarget = targetTransformInSight;
				}
			}
		}
		
		animator.SetLookAtWeight(lookWeight);
		float lookWeightFinal = 0;
		
		// If it is idle time after shooting some intruder
		if (idleAfterShootingTimeCounter > 0)
		{
			sawOrHeardTargetPerson = false;
			status = Status.Idle;
		}
		// If see or hear a person
		else if (hitsInPerson > 0 || ears.hearSteps) 
		{
			if (hitsInPerson > 0)
			{
				targetPosition = targetTransformInSight.position;
				lookWeightFinal = 1f;
			}
			else if (ears.hearSteps)
			{
				targetPosition = ears.soundSourcePosition;
			}
			
			sawOrHeardTargetPerson = true;
			status = Status.Following;
			agent.speed = runSpeed;
			
			alertSign.AttachTo(new Vector3(targetPosition.x, 0, targetPosition.z));
			visionCone.status = FOV2DVisionCone.Status.Alert;
		}
		
		lookWeight = Mathf.Lerp(lookWeight, lookWeightFinal, Time.deltaTime * 3f);
		if (shootTarget)
		{
			animator.SetLookAtPosition(shootTarget.position);
		}
		
		// Use a state machine for a behaviour actions of our "AI".
		
		// Idle
		if (status == Status.Idle) 
		{
			if (waypointManager.TargetPoint)
			{
				float distanceToTargetWaypoint = Vector3.Distance(waypointManager.TargetPoint.transform.position, transform.position);
				
				// Go to the base point if not yet there
				if (distanceToTargetWaypoint > 1.5f)
				{
					targetPosition = waypointManager.TargetPoint.transform.position;
					status = Status.Following;
				}
				else
				{
					waypointManager.SetNextPoint();
				}
			}
			
			agent.speed = walkSpeed;
			alertSign.Detach();
			visionCone.status = FOV2DVisionCone.Status.Idle;
		}
		// Following the target position
		else if (status == Status.Following) 
		{
			float distanceToTarget = Vector3.Distance(targetPosition, transform.position);

			
			// If close enought to shoot a laser beam in the target person
			if (distanceToTarget <= shootDistance && targetTransformInSight && lookWeight >= 0.9f) 
			{
				targetTransformInSight.gameObject.SendMessage("Shot");
				idleAfterShootingTimeCounter = idleTimeAfterShooting;
			} 
			// If the target position has reached
			else if (distanceToTarget < 1.5f)
			{
				// Follow the target
				agent.destination = transform.position;
				
				// If previously saw the target person
				if (sawOrHeardTargetPerson)
				{
					status = Status.Searching;
					
					sawOrHeardTargetPerson = false;
				}
				// If still searching for the target person
				else if (searchTimeCurrent > 0)
				{
					status = Status.Searching;
				}
				else
				{
					status = Status.Idle;
				}
			}
			// Go for the target position
			else
			{
				// If found the target or the searching time has expired
				if (sawOrHeardTargetPerson && searchTimeCurrent > 0)
				{
					searchTimeCurrent = 0;
					
					visionCone.status = FOV2DVisionCone.Status.Idle;
				}
				else if (searchTimeCurrent >= searchTime)
				{
					searchTimeCurrent = 0;
					status = Status.Idle;
				}
				// If the searching timer on — add new delta-time to it
				else if (searchTimeCurrent > 0)
				{
					searchTimeCurrent += Time.deltaTime;
				}
				
				// Follow the target position
				agent.destination = targetPosition;
			}
		}
		// Searching for the target person
		else if (status == Status.Searching) 
		{
			// If the searching time has expired
			if (searchTimeCurrent >= searchTime)
			{
				status = Status.Idle;
				searchTimeCurrent = 0;
			}
			else
			{
				if (searchTimeCurrent == 0)
				{
					centerOfSearchArea = targetPosition + (transform.forward * searchRadius);
				}

				searchTimeCurrent += Time.deltaTime;
				
				NavMeshPath newPath = new NavMeshPath();;
				int maxAttempts = 100;
				int attempts = 0;
				
				// Find a new random target position in the radius.
				do
				{
					targetPosition = new Vector3(
						centerOfSearchArea.x + Random.Range(-searchRadius,searchRadius), 
						centerOfSearchArea.y, 
						centerOfSearchArea.z + Random.Range(-searchRadius,searchRadius));
	
					attempts++;
					
					if (attempts >= maxAttempts)
					{
						targetPosition = transform.position;
						break;
					}
				}
				// The target position should be accessible on the NavMesh
				while(agent.CalculatePath(targetPosition, newPath) == false);
				
				agent.destination = targetPosition;
				status = Status.Following;
				
				alertSign.AttachTo(new Vector3(targetPosition.x, 0, targetPosition.z));
			}
			
		}
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (targetPosition, 0.5f);
	}
}
