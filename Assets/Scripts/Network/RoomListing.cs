using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomListing : Photon.MonoBehaviour {

    public string roomName;
    public Text roomLabelText;
    public Text occupantText;
    public Button joinButton;


    private NetworkLobby parent;


    public void Initialize(string roomName, NetworkLobby parent) {
        this.roomName = roomName;
        this.parent = parent;

        roomLabelText.text = roomName;
    }



    public void ShowOccupants(int players) {
        if (players >= 2) {
            occupantText.text = "Room Full";
            joinButton.interactable = false;
        }
        else {
            occupantText.text = players.ToString() + "/2" + " Players";
            joinButton.interactable = true;
        }

    }


    public void Join() {
        PhotonNetwork.JoinRoom(roomName);
        SceneManager.LoadScene("Main");

    }

}
