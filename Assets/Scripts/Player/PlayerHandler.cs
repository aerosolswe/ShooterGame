using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerHandler : NetworkBehaviour {
    
    private CharacterController controller;
    private PlayerMovement movement;
	private PlayerShoot playerShoot;

	private NetworkManager networkManager;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	public const int maxHealth = 100;

	[SyncVar]
	private int health = maxHealth;

	[SyncVar]
	private bool isDead = false;

	[SyncVar]
	private bool isInvurnable = false;


	public bool IsDead {
		get { return isDead; }
		protected set { isDead = value; }
	}

	public bool IsInvurnable {
		get { return isInvurnable; }
		protected set { isInvurnable = value; }
	}

    public void Start() {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
		playerShoot = GetComponent<PlayerShoot>();
		networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

		if(!isLocalPlayer) {
			DisableComponents();
			AssignRemoteLayer();
		}

		SetDefaults();
    }

	public override void OnStartClient () {
		base.OnStartClient ();

		string netId = GetComponent<NetworkIdentity>().netId.ToString();

		GameManager.RegisterPlayer(netId, this);

		/*if(isLocalPlayer)
			GameManager.instance.SetPlayerAlias(this.transform.name);*/

	}

	void OnDisable() {
		GameManager.UnRegisterPlayer(transform.name);
	}

	void DisableComponents() {
		movement.enabled = false;
		playerShoot.enabled = false;
		GetComponentInChildren<Camera>().gameObject.SetActive(false);
	}

	void AssignRemoteLayer() {
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
		for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
		}
	}

	public void SetDefaults() {
		health = maxHealth;
		isDead = false;
	}
	
	public void TakeDamage(int amount, Player attacker) {
		if(isDead || isInvurnable || !isServer)
			return;

		health -= amount;

		if(health <= 0) {
			health = maxHealth;

			string victim = GetComponent<Player>().alias;
			string killer = "environment";

			if(attacker != null)
				killer = attacker.alias;

			RpcDie(killer, victim);
		}

	}

	[ClientRpc]
	private void RpcDie(string killer, string victim) {

		if(killer == "environment")
			UIHandler.instance.deathCounter.AddDeath(victim, "", 3, "killed himself..");
		else 
			UIHandler.instance.deathCounter.AddDeath(killer, victim, 3);
		
		/*Collider[] colliders = GetComponentsInChildren<Collider>();
		foreach(Collider collider in colliders)
			collider.enabled = false;
		transform.FindChild("Capsule").gameObject.SetActive(false);*/

		if(isLocalPlayer) {
			WarpPlayer(networkManager.GetStartPosition().position, networkManager.GetStartPosition().rotation);
			//isDead = false;
		}
		
		//Collider[] colliders = GetComponentsInChildren<Collider>();
		/*foreach(Collider collider in colliders)
			collider.enabled = true;
		transform.FindChild("Capsule").gameObject.SetActive(true);*/
	}

	public void WarpPlayer(Vector3 pos, Quaternion rot) {
		transform.position = pos;
		transform.rotation = rot;
	}
    
}
