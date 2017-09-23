using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

public class StartGameButton : MonoBehaviour {

    public Text buttonText;
    public GameObject buttonObject;
    public bool testing = false;

    private Deck[] allMyDecks;
    private Player player;


    void Start () {
        player = GetComponent<Player>();

        if (!player.player2) {
            buttonText.text = "Waiting For Opponent";
        }

        //Grid.EventManager.RegisterListener(Constants.GameEvent.DeckSelected, OnDeckSelected);
    }


    //private void OnDeckSelected(EventData data) {
    //    Player p = data.GetMonoBehaviour("Player") as Player;

    //    if(p != player && !player.player2) {
    //        buttonText.text = "Start Game";
    //    }

    //}


    void OnPhotonPlayerConnected(PhotonPlayer player) {
        if (!this.player.player2) {
            buttonText.text = "Start Game";
        }
    }

    public void BeginStartGameProcess() {
        allMyDecks = GetComponentsInChildren<Deck>();
        player.StartCoroutine(player.StartGame());

        if (buttonText.text == "Start Game" || testing) {
            foreach (Deck d in allMyDecks) {
                d.StartCoroutine(d.SpawnAllCards());
            }
            buttonObject.SetActive(false);
        }
    }


}