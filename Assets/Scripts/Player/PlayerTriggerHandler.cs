using UnityEngine;
using System.Collections;

public class PlayerTriggerHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other) {

		if(other.gameObject.tag == "HurtBox") {
			GetComponent<PlayerHandler>().TakeDamage(100, null);
		}

		if(other.gameObject.tag == "ForceBox") {
			GetComponent<PlayerMovement>().ApplyForce(other.gameObject.GetComponent<ForceBox>().force);
		}

		if(other.gameObject.tag == "TPBox") {
			GetComponent<PlayerHandler>().WarpPlayer(other.gameObject.GetComponent<TPBox>().position, other.transform.rotation);
			if(other.gameObject.GetComponent<TPBox>().resetVelocity)
				GetComponent<PlayerMovement>().velocity = Vector3.zero;
		}

	}

	
	void OnTriggerStay(Collider other) {
		// KillBox

	}
}
