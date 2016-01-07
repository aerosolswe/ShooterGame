using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathHandler : MonoBehaviour {

	public Text killerText;
	public Text killText;
	public Text victimText;

	public void Initialize() {
		killerText = GetComponentsInChildren<Text>()[0];
		killText = GetComponentsInChildren<Text>()[1];
		victimText = GetComponentsInChildren<Text>()[2];
	}

	public void StartDeathCountdown(float time) {
		StartCoroutine(DeathCountdown(time));
	}

	IEnumerator DeathCountdown(float time) {
		yield return new WaitForSeconds(time);

		Destroy(this.gameObject);
	}
}
