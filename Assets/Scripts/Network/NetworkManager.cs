using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : Photon.MonoBehaviour {

    private const string VERSION = "v0.0.0.1";
    public static NetworkManager _networkManager;

    public string roomName = "Mario's Hammer2";
    public string playerPrefabName = "NetworkPlayer";
    //public string player2Name = "NetworkPlayer2";
    //public Transform spawnPoint;
    public Transform[] spawnpoints;
    //public GameObject mainMenu;
    //public GameObject endGameScreen;
    public bool firstPlayer;

    private int spawnIndex = 0;

    [SerializeField] Text connetionText;
    [SerializeField] Camera sceneCamera;

    private GameObject player1;
    private GameObject player2;


    void Awake() {
        _networkManager = this;
    }

    void Start() {
        ConnectToGame();
    }

    public void ConnectToGame() {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        //mainMenu.SetActive(false);
    }

    private void Update() {
        connetionText.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    private void OnJoinedLobby() {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom() {

        sceneCamera.enabled = true;

        StartSpawnProcess(0.5f);
    }

    private void StartSpawnProcess(float respawnTime) {
        sceneCamera.enabled = true;

        StartCoroutine(SpawnPlayer(respawnTime));
    }

    private IEnumerator SpawnPlayer(float respawnTime) {
        yield return new WaitForSeconds(respawnTime);

        if (!firstPlayer) {
            spawnIndex = 0;
            player1 = PhotonNetwork.Instantiate(playerPrefabName, spawnpoints[spawnIndex].position, spawnpoints[spawnIndex].rotation, 0);
            player1.GetComponent<Player>().myTurn = true;
            player1.transform.localPosition = Vector3.zero;
            RPCFirstPlayerArrived(PhotonTargets.AllBufferedViaServer);
        }
        else {
            spawnIndex = 1;
            player2 = PhotonNetwork.Instantiate(playerPrefabName, spawnpoints[spawnIndex].position, spawnpoints[spawnIndex].rotation, 0);
            player2.GetComponent<Player>().RPCAmPlayer2(PhotonTargets.All);
            player2.transform.localPosition = Vector3.zero;
        }

        sceneCamera.enabled = false;

    }



    #region RPCs

    public void RPCFirstPlayerArrived(PhotonTargets targets) {
        photonView.RPC("FirstPlayerArrived", targets);
    }

    [PunRPC]
    public void FirstPlayerArrived() {
        firstPlayer = true;
    }

    #endregion

}