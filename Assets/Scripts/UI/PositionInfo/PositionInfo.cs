using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PositionInfo : MonoBehaviour {

	public Text jumpText;
	public Text velocityText;

	void Update () {
		float jumpLength = 0;
		float velocity = 0;

		PlayerHandler ph = GameManager.instance.localPlayer;

		if(ph != null) {
			PlayerMovement pm = ph.GetComponent<PlayerMovement>();

			jumpLength = pm.jumpLength;
			velocity = pm.currentVelocity;
		}
			

		jumpText.text = "Jump length: " + jumpLength;
		velocityText.text = "Velocity: " + velocity;
	}
}
