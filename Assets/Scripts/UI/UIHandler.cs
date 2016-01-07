using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class UIHandler : MonoBehaviour {

	public static UIHandler instance;

	private NetworkManager networkManager;

	public GameObject mainMenu;
	public GameObject ingameMenu;
	public DeathCounter deathCounter;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(this.gameObject);

		networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		SetIp("localhost");

		ShowMainMenu(true);
	}

	public void StartServer() {
		bool startedServer = networkManager.StartServer();
	}

	public void StartHost() {
		networkManager.StartHost();
	}

	public void StartClient() {
		networkManager.StartClient();
	}

	public void Disconnect() {
		networkManager.StopHost();
	}

	public void StopServer() {
		networkManager.StopServer();
	}

	public void StartLobby() {
		
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void SetIp(string ip) {
		networkManager.networkAddress = ip;
	}

	public void ShowMainMenu(bool value) {
		mainMenu.SetActive(value);
		ingameMenu.SetActive(!value);
	}

	public void HideHud() {
		mainMenu.SetActive(false);
		ingameMenu.SetActive(false);
	}
}
