using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelector : MonoBehaviour {


    [Header("Default Deck")]
    public GameObject defaultGrimoire;
    public GameObject defaultDomain;
    public GameObject defaultSoulCrypt;

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
