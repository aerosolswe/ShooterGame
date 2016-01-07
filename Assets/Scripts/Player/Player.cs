using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	[SyncVar]
	public string alias;

	void Start() {
		CmdChangeAlias(GameManager.instance.alias);
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
