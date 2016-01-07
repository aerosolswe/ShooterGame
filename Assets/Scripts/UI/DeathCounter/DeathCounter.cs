using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeathCounter : MonoBehaviour {

	public GameObject deathObject;

	public List<GameObject> deaths = new List<GameObject>();

	void Start() {
		
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.F1)) {
			AddDeath("ayy", "lmao");
		}
	}

	public void AddDeath(string killer, string victim, float showTime = 3, string kill = "pwned") {
		GameObject d = Instantiate(deathObject);
		d.transform.SetParent(this.transform);
		d.GetComponent<RectTransform>().SetParent(this.transform);

		DeathHandler dh = d.GetComponent<DeathHandler>();
		dh.Initialize();
		dh.killerText.text = killer;
		dh.victimText.text = victim;
		dh.killText.text = kill;

		d.SetActive(true);

		dh.StartDeathCountdown(showTime);
	}
}
