using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	[SyncVar]
	public string alias;

	[SyncVar]
	public int score;

	[SyncVar]
	public int ping;

	private NetworkClient client;

	void Start() {
		CmdChangeAlias(GameManager.instance.alias);

		if(isLocalPlayer) {
			client = GameManager.instance.networkManager.client;

			InvokeRepeating("UpdatePing", 1, 1);
		}
	}

	void UpdatePing() {
		int rtt = client.GetRTT();

		CmdSetPing(rtt);
	}

	[Command]
	public void CmdSetPing(int ping) {
		this.ping = ping;
	}

	[Command]
	public void CmdAddScore(int scoreToBeAdded) {
		this.score += scoreToBeAdded;
	}

	[Command]
	public void CmdChangeAlias(string alias) {
		if(alias.Length == 0) {
			this.alias = "player";
			return;
		}

		this.alias = alias;
	}
}
