using UnityEngine;
using System.Collections;

public class FootTrigger : MonoBehaviour
{	
	private Animator animator;
	
	void Start ()
	{
		animator = transform.parent.gameObject.GetComponent<Animator>();
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name == "JumpTrigger")
		{
			animator.SetBool("JumpDown", true);	
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.name == "JumpTrigger")
		{
			animator.SetBool("JumpDown", false);
		}
	}
}
