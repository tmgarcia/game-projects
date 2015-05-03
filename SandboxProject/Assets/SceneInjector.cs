using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SceneInjector : MonoBehaviour {

    public NetworkLobbyManager manager;

    public string onlineScene = "Game";
    public string offlineScene = "Lobby";

	// Use this for initialization
	void Start () {
        manager.playScene = onlineScene;
        manager.lobbyScene = offlineScene;
	}
}
