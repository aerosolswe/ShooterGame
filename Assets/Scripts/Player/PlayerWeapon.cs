using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	public string name = "Laser";

	public int damage = 50;

	public float bulletTravelSpeed = 50;

	public float fireRate = 1;

	public bool auto = false;

	public float range = 100f;

}
