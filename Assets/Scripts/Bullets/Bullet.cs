using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int damage;
	public Player attacker;

	void OnCollisionEnter(Collision collision) {
		var hit = collision.gameObject;
		var hitPlayer = hit.GetComponent<PlayerHandler>();


		if (hitPlayer != null) {
			hitPlayer.TakeDamage(damage, attacker);
		}

		Destroy(this.gameObject);
	}
}
