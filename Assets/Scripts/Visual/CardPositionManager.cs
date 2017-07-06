using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPositionManager : MonoBehaviour {

    //public Constants.DeckType collectionLocation;

    public List<CardPosition> cardPositions = new List<CardPosition>();
    public float offset = 0.5f;


    void Start () {
        for (int i = 0; i < cardPositions.Count; i++) {
            cardPositions[i].cardPosition.localPosition = new Vector3(
                cardPositions[i].cardPosition.localPosition.x, 
                cardPositions[i].cardPosition.localPosition.y, 
                cardPositions[i].cardPosition.localPosition.z - ((i + 1) * offset) );
        }
    }

    public Transform GetFirstEmptyCardPosition() {
        Transform result = null;

        for (int i = 0; i < cardPositions.Count; i++) {
            if (!cardPositions[i].filled) {
                result = cardPositions[i].cardPosition;
                cardPositions[i].filled = true;
                break;
            }
        }

        return result;
    }

    public Transform AssignSpecificPosition(Transform testlocation) {

        for(int i = 0; i < cardPositions.Count; i++) {
            if (cardPositions[i].cardPosition == testlocation && cardPositions[i].filled) {
                Debug.LogWarning("[Card Position Manager] The position in question is alreay filled. Returning Null");
                return null;
            }

            if(cardPositions[i].cardPosition == testlocation && !cardPositions[i].filled) {
                cardPositions[i].filled = true;
                return testlocation;
            }
        }

        return null;
    }

    public bool IsCollectionFull() {
        bool result = true;

        for(int i = 0; i < cardPositions.Count; i++) {
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
                break;
            }
        }

    }


    [System.Serializable]
    public class CardPosition {
        public Transform cardPosition;
        public bool filled;
    }
}
