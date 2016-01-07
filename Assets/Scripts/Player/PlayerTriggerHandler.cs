using UnityEngine;
using System.Collections;

public class PlayerTriggerHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other) {

		if(other.gameObject.tag == "HurtBox") {
			GetComponent<PlayerHandler>().TakeDamage(100, null);
		}

	}

	
	void OnTriggerStay(Collider other) {
		// KillBox

	}
}
