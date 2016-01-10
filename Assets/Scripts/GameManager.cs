using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	private const string PLAYER_ID_PREFIX = "Player ";

	private static Dictionary<string, PlayerHandler> players = new Dictionary<string, PlayerHandler> ();

	public PlayerHandler localPlayer = null;

	public NetworkManager networkManager;

	public string alias = "player";

	public void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	public void SetAlias(string alias) {
		this.alias = alias;
	}

	public static void RegisterPlayer(string netId, PlayerHandler player) {
		string playerId = PLAYER_ID_PREFIX + netId;

		players.Add(playerId, player);
		player.transform.name = playerId;

	}

	public static void UnRegisterPlayer(string playerId) {
		players.Remove(playerId);
	}

	public static PlayerHandler GetPlayer(string id) {

		if(players.ContainsKey(id))
			return players[id];

		return null;
	}

	public Dictionary<string, PlayerHandler> GetPlayers() {
		return players;
	}
}
