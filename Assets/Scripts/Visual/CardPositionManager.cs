using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPositionManager : MonoBehaviour {

    //public Constants.DeckType collectionLocation;

    public enum ManagerLocation {
        Hand,
        Battlefield,
        Mulligan,
        Support
    }

    public ManagerLocation managerLocation;
    public List<CardPosition> cardPositions = new List<CardPosition>();
    public float offset = 0.5f;
    public Player owner;


    void Start() {
        owner = GetComponentInParent<Player>();

        for (int i = 0; i < cardPositions.Count; i++) {
            cardPositions[i].cardPosition.localPosition = new Vector3(
                cardPositions[i].cardPosition.localPosition.x,
                cardPositions[i].cardPosition.localPosition.y,
                cardPositions[i].cardPosition.localPosition.z - ((i + 1) * offset));
        }
    }

    public Transform GetFirstEmptyCardPosition(CardVisual card) {
        Transform result = null;

        for (int i = 0; i < cardPositions.Count; i++) {
            if (!cardPositions[i].filled) {
                result = cardPositions[i].cardPosition;
                cardPositions[i].filled = true;
                cardPositions[i].card = card;

                if(managerLocation == ManagerLocation.Battlefield)
                    RPCBroadcastcardPositionIndex(PhotonTargets.Others, i, card);

                break;
            }
        }

        return result;
    }

    public Transform AssignSpecificPosition(Transform testlocation, CardVisual card) {

        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardPositions[i].cardPosition == testlocation && cardPositions[i].filled) {
                Debug.LogWarning("[Card Position Manager] The position in question is alreay filled. Returning Null");
                return null;
            }

            if (cardPositions[i].cardPosition == testlocation && !cardPositions[i].filled) {
                cardPositions[i].filled = true;
                cardPositions[i].card = card;

                if (managerLocation == ManagerLocation.Battlefield)
                    RPCBroadcastcardPositionIndex(PhotonTargets.Others, i, card);
                return testlocation;
            }
        }

        return null;
    }

    public bool IsCollectionFull() {
        bool result = true;

        for (int i = 0; i < cardPositions.Count; i++) {
            if (!cardPositions[i].filled) {
                result = false;
                break;
            }
        }

        return result;
    }

    public void ReleaseCardPosition(Transform currentPos) {
        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardPositions[i].cardPosition == currentPos) {
                cardPositions[i].filled = false;
                cardPositions[i].card = null;

                if(managerLocation == ManagerLocation.Battlefield)
                    RPCNullCardPosition(PhotonTargets.Others, i);

                break;
            }
        }

    }


    public CardVisual GetCardToTheLeft(CardVisual targetCard) {
        CardVisual result = null;

        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardPositions[i].card == targetCard) {
                if (i == 0)
                    return null;
                else {
                    result = cardPositions[i - 1].card;
                    break;
                }

            }
        }
        return result;
    }

    public CardVisual GetCardToTheRight(CardVisual targetCard) {
        CardVisual result = null;

        for (int i = 0; i < cardPositions.Count; i++) {
            if (cardPositions[i].card == targetCard) {
                if (i == cardPositions.Count - 1)
                    return null;
                else {
                    result = cardPositions[i + 1].card;
                    break;
                }

            }
        }
        return result;

    }


    private void RPCBroadcastcardPositionIndex(PhotonTargets targets, int index, CardVisual card) {
        int cardID = card.photonView.viewID;

        owner.photonView.RPC("BroadcastCardPosition", targets, index, cardID);

    }

    private void RPCNullCardPosition(PhotonTargets targets, int index) {

        owner.photonView.RPC("BroadcastNullCardPosition", targets, index);
    }



    [System.Serializable]
    public class CardPosition {
        public Transform cardPosition;
        public bool filled;
        public CardVisual card;
    }
}
