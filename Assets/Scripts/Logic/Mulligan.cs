using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mulligan : MonoBehaviour {


    public GameObject endTurnButton;
    public static bool choosingMulligan = true;
    public Player owner;

    private List<CardVisual> rejectedCards = new List<CardVisual>();


    void Start () {
        owner = GetComponentInParent<Player>();
	}


    public void ConfirmMulligan() {
        owner.gameHasStarted = true;
        if (owner.myTurn) {
            owner.gameState = Player.GameStates.Refresh;
        }

        for(int i = 0; i < owner.myHand.activeCards.Count; i++) {
            if (owner.myHand.activeCards[i].isMulligand) {
                rejectedCards.Add(owner.myHand.activeCards[i]);
            }

            if (!owner.myHand.activeCards[i].isMulligand) {
                Transform handPos = owner.handManager.GetFirstEmptyCardPosition(owner.myHand.activeCards[i]);
                owner.mulliganManager.ReleaseCardPosition(owner.myHand.activeCards[i].handPos);
                owner.myHand.activeCards[i].handPos = handPos;
                owner.myHand.activeCards[i].RPCSetCardAciveState(PhotonTargets.All, true);
            }

            owner.myHand.activeCards[i].SetCardActiveState(true);
        }

        endTurnButton.SetActive(true);

        CleanUp(rejectedCards);

    }


    void CleanUp(List<CardVisual> cards) {
        choosingMulligan = false;

        for(int i = 0; i < cards.Count; i++) {
            owner.mulliganManager.ReleaseCardPosition(cards[i].handPos);
            cards[i].currentDeck.RPCTransferCard(PhotonTargets.All, cards[i], cards[i].owner.activeGrimoire.GetComponent<Deck>());
            owner.activeGrimoire.GetComponent<Deck>().DrawCard();
        }

        gameObject.SetActive(false);

        //foreach (GameObject card in cards) {
        //    owner.mulliganLocations.GetComponent<HandManager>().handInfo[card.GetComponent<CardModel>().handPos] = false;
        //    card.GetComponent<CardModel>().SendCardToGrimoire();
        //    //Debug.Log(card + " is tossed back");
        //    owner.activeGrimoire.GetComponent<Deck>().DrawCard();
        //}

        //if (owner.player2) {

        //    owner.activeGrimoire.GetComponent<Deck>().DrawCard();
        //    GameObject coin = Resources.Load("Remnant") as GameObject;
        //    CardAbility.UniversalSpawnToken(owner, coin, true);

        //}

        //gameObject.SetActive(false);
    }




}
