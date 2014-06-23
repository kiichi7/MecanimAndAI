using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public Sign selectSign;
	[System.NonSerialized]					
	public float lookWeight;
	
	private Animator animator;
	private StepsSound stepsSound;
	private bool canTurnOnSpot;
	private bool teleported;
	private Transform playerBase;
	//private Transform head;
	//private Transform guardTarget;
	
	void Start() 
	{
		animator = GetComponentInChildren<Animator>();
		stepsSound = GetComponentInChildren<StepsSound>();
		
		playerBase = GameObject.Find("PlayerBase").transform;
		//head = transform.Find("ShootTarget");
		//guardTarget = GameObject.Find("GuardTarget").transform;
	}
	
	void Update() 
	{
		float horizontalInput = Input.GetAxis("Horizontal");				
		float verticalInput = Input.GetAxis("Vertical");	
		
		animator.SetFloat("Speed", verticalInput);									
		animator.SetFloat("Direction", horizontalInput);
		animator.SetFloat("AngleDirection", horizontalInput * 180f * 0.7f);
		
		if (verticalInput > 0.01f)
		{
			stepsSound.transform.localScale = new Vector3(stepsSound.maxScale * verticalInput, 1, 1);
		}
		else
		{
			stepsSound.transform.localScale = new Vector3(stepsSound.mingScale, 1, 1);
		}
		
		if (horizontalInput != 0 && verticalInput < 0.01f)
		{
			canTurnOnSpot = true;
		}
		else
		{
			canTurnOnSpot = false;
		}
		
		animator.SetBool("Turn", canTurnOnSpot);
		
		selectSign.AttachTo(transform.position);
		selectSign.Rotation = transform.rotation;
		
		AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
		
		if (currentState.IsName("Base Layer.WakeUp") && !teleported)
		{
			transform.position = playerBase.transform.position;
			transform.rotation = playerBase.transform.rotation;
			teleported = true;
		}
		
        if (nextState.IsName("Base Layer.Die"))
        {                
            animator.SetBool("Die", false);
        }	
	}
	
	void FixedUpdate()
	{
		// TODO: fix a bug when player's head shaking in user build versions (Not in Editor player) when he looks directly at the robot.
		/*
		animator.SetLookAtWeight(lookWeight);
		head.LookAt(guardTarget.position);
		RaycastHit hit;
		float lookWeightFinal = 0f;
		
		// If guard can be seen — look in his direction
		if (Physics.Raycast(head.position, head.forward, out hit))
		{
			if (hit.transform == guardTarget.parent)
			{
				lookWeightFinal = 1f;
			}
		}
		lookWeight = Mathf.Lerp(lookWeight, lookWeightFinal, Time.deltaTime * 3f);
		animator.SetLookAtPosition(guardTarget.position);
		*/
	}
	
	void Shot()
	{
		AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
		
		if (!currentState.IsName("Base Layer.Die") && !nextState.IsName("Base Layer.Die"))
		{
			animator.SetBool("Die", true);
			teleported = false;
		}
	}
}
