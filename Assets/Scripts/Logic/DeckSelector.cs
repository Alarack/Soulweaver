using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelector : MonoBehaviour {


    [Header("Default Deck")]
    public GameObject defaultGrimoire;
    public GameObject defaultDomain;
    public GameObject defaultSoulCrypt;

    [Header("Rillock")]
    public GameObject rillockGrimoire;
    public GameObject rillockDomain;
    //public GameObject rillockSoulcrypt;

    [Header("Wardens")]
    public GameObject wardensGrimoire;
    public GameObject wardensDomain;

    [Header("Keepers")]
    public GameObject keepersGrimoire;
    public GameObject keepersDomain;

    [Space(10)]
    public GameObject startGameButton;

    private Player player;


    void Start () {
        player = GetComponentInParent<Player>();
	}

	void Update () {
		
	}

    public void AssignDefaultDeck() {
        DeckAssignmentHelper(defaultGrimoire, defaultDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
        //startGameButton.SetActive(true);
        //gameObject.SetActive(false);

    }

    public void AssignRillockDeck() {
        DeckAssignmentHelper(rillockGrimoire, rillockDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignWardensDeck() {
        DeckAssignmentHelper(wardensGrimoire, wardensDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignKeeperDeck() {
        DeckAssignmentHelper(keepersGrimoire, keepersDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }



    private void ShowStartGame() {
        startGameButton.SetActive(true);
        gameObject.SetActive(false);
    }

    private void DeckAssignmentHelper(GameObject grimoire, GameObject domain, GameObject soulcrypt) {
        for(int i = 0; i < player.deckInfo.Count; i++) {
            switch (player.deckInfo[i].deckType) {
                case Constants.DeckType.Grimoire:
                    player.deckInfo[i].deck = grimoire;
                    break;

                case Constants.DeckType.Domain:
                    player.deckInfo[i].deck = domain;
                    break;

                case Constants.DeckType.SoulCrypt:
                    player.deckInfo[i].deck = soulcrypt;
                    break;
            }
        }
    }
}
