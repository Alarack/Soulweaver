using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkLobby : Photon.MonoBehaviour {

    private const string VERSION = "v0.0.0.1";

    public RoomListing roomListingTemplate;
    public RectTransform roomListingContainer;
    public InputField inputField;

    [SerializeField]
    private Text connetionText;

    private List<RoomListing> allRooms = new List<RoomListing>();

    void Start() {
        PhotonNetwork.ConnectUsingSettings(VERSION);
    }


    void OnConnectedToMaster() {
        //Debug.Log("OnConnectedToMaster()");

        TypedLobby defaultLobby = new TypedLobby("Soulweaver", LobbyType.Default);

        PhotonNetwork.JoinLobby(defaultLobby);

    }

    void OnReceivedRoomListUpdate() {
        PopulateRoomList();
    }

    private void Update() {
        if (connetionText != null)
            connetionText.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    private void OnJoinedLobby() {
        PopulateRoomList();       
    }

    private void PopulateRoomList() {
        DestroyAllRoomListings();

        RoomInfo[] games = PhotonNetwork.GetRoomList();

        for (int i = 0; i < games.Length; i++) {
            CreateRoomListing(games[i]);
        }
    }



    private void CreateRoomListing(RoomInfo roomInfo) {
        GameObject roomListing = Instantiate(roomListingTemplate.gameObject) as GameObject;
        roomListing.transform.SetParent(roomListingContainer, false);
        roomListing.gameObject.SetActive(true);

        RoomListing roomScript = roomListing.GetComponent<RoomListing>();

        roomScript.Initialize(roomInfo.Name, this);

        roomScript.ShowOccupants(roomInfo.PlayerCount);

        allRooms.Add(roomScript);
    }

    private void DestroyAllRoomListings() {
        for (int i = 0; i < allRooms.Count; i++) {
            Destroy(allRooms[i].gameObject);
        }

    }


    public void OnCreateClick() {
        if (inputField.text == "") {
            return;
        }

        CreateRoom(inputField.text);

    }




    public void CreateRoom(string roomName) {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);

        SceneManager.LoadScene("Main");
    }


}
