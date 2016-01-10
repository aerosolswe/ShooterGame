using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class Scoreboard : MonoBehaviour {

	/*
		TODO: Only update the list of players when a player joins/leaves instead of pressing Tab
		TODO: Sort players by score
	*/

	public List<PlayerHandler> players;
	public List<GameObject> scores;

	public GameObject scoreObject;
	public GameObject content;

	public GameObject scrollView;

	public float startY = -30;

	private bool down = false;

	void Start() {
		players = new List<PlayerHandler>();
		scores = new List<GameObject>();
	}

	void Update() {
		if(CrossPlatformInputManager.GetButtonDown("Scoreboard")) {
			UpdateList();
			UpdateScores();
			ShowScoreboard();
			down = true;
		}

		if(CrossPlatformInputManager.GetButtonUp("Scoreboard")) {
			HideScoreboard();
			down = false;
		}

		if(!down)
			ClearScores();
	}

	public void UpdateList() {
		players.Clear();
		players.AddRange(GameManager.instance.GetPlayers().Values);
	}

	public void UpdateScores() {
		float yPos = startY;
		for(int i = 0; i < players.Count; i++) {
			GameObject score = Instantiate(scoreObject);
			score.transform.SetParent(content.transform, false);
			score.transform.localPosition = new Vector3(score.transform.localPosition.x, yPos, 0);
			score.SetActive(true);

			Player p = players[i].GetComponent<Player>();

			score.transform.FindChild("Name").GetComponent<Text>().text = p.alias;
			score.transform.FindChild("Score").GetComponent<Text>().text = "" + p.score;
			score.transform.FindChild("Ping").GetComponent<Text>().text = "" + p.ping;

			scores.Add(score);

			yPos -= 30;
		}
	}

	public void ShowScoreboard() {
		scrollView.SetActive(true);
	}

	public void HideScoreboard() {
		scrollView.SetActive(false);
	}

	public void ClearScores() {
		for(int i = 0; i < scores.Count; i++) {
			Destroy(scores[i]);
			scores.RemoveAt(i);
		}
	}
}
