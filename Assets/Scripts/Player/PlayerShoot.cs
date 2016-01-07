using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerShoot : NetworkBehaviour {

	public PlayerWeapon weapon;
	public GameObject bulletPrefab;

	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	void Start() {
		cam = GetComponentInChildren<Camera>();
	}

	void Update() {
		if(CrossPlatformInputManager.GetButtonDown("Shoot")) {
			//Shoot(transform.name, cam.transform.position, cam.transform.forward, mask, weapon);
			CmdFire(transform.name, cam.transform.position, cam.transform.forward, cam.transform.rotation, 25, weapon);
		}
	}

	[Command]
	void CmdFire(string shooter, Vector3 pos, Vector3 forward, Quaternion rotation, int damage, PlayerWeapon weapon) {
		var bullet = (GameObject)Instantiate(bulletPrefab, pos + forward, rotation);
		bullet.SetActive(false);

		bullet.GetComponent<Rigidbody>().velocity = forward * 50;
		bullet.transform.rotation = rotation;

		Player killer = GameManager.GetPlayer(shooter).GetComponent<Player>();

		bullet.GetComponent<Bullet>().damage = damage;
		bullet.GetComponent<Bullet>().attacker = killer;
		//bullet.layer = LayerMask.

		bullet.SetActive(true);
		NetworkServer.Spawn(bullet);


		Destroy(bullet, 5f);
	}

	[Client]
	void Shoot(string shooter, Vector3 pos, Vector3 forward, LayerMask mask, PlayerWeapon weapon) {
		RaycastHit hit;

		Debug.DrawRay(pos, forward * 10, Color.red, 100, true);
		if(Physics.Raycast(pos, forward, out hit, weapon.range, mask)) {
			if(hit.collider.tag == "Player") {
				CmdPlayerShot(hit.collider.name, weapon.damage, shooter);
			}
		}
	}

	[Command]
	void CmdPlayerShot(string playerId, int damage, string killerid) {
		PlayerHandler player = GameManager.GetPlayer(playerId);
		Player killer = GameManager.GetPlayer(killerid).GetComponent<Player>();

		player.TakeDamage(damage, killer);
	}
}
