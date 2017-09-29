using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom : Photon.MonoBehaviour {

    public static List<Deck> _allDecks = new List<Deck>();

    public string playerPrefabName = "NetworkPlayer";
    public Transform[] spawnpoints;
    //public GameObject mainMenu;
    //public GameObject endGameScreen;
    public bool firstPlayer;

    private int spawnIndex = 0;

    [SerializeField]
    Camera sceneCamera;

    private GameObject player1;
    private GameObject player2;

    private void OnJoinedRoom() {

        Debug.Log("OnJoinedRoom() : You Have Joined a Room : " + PhotonNetwork.room.Name);

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
            //player1.GetComponent<Player>().RPCCheckOpponents(PhotonTargets.AllBufferedViaServer);
            //player2.GetComponent<Player>().RPCCheckOpponents(PhotonTargets.AllBufferedViaServer);
        }

        sceneCamera.enabled = false;

    }



    public void RPCFirstPlayerArrived(PhotonTargets targets) {
        photonView.RPC("FirstPlayerArrived", targets);
    }

    [PunRPC]
    public void FirstPlayerArrived() {
        firstPlayer = true;
    }

}
