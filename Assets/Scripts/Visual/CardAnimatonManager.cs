using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAnimatonManager : MonoBehaviour {


    public List<TextAnimationInfo> textAnimInfo = new List<TextAnimationInfo>();


    private CardVisual card;

	void Awake () {
        card = GetComponentInParent<CardVisual>();
	}

	void Update () {
		
	}




    public void BounceText(Constants.CardStats stat) {
 
        if (card.currentDeck.decktype != Constants.DeckType.Battlefield && card.currentDeck.decktype != Constants.DeckType.Hand)
            return;


        for(int i = 0; i < textAnimInfo.Count; i++) {
            if(textAnimInfo[i].stat == stat) {
                if (textAnimInfo[i].rect.gameObject.activeInHierarchy) {
                    textAnimInfo[i].rect.GetComponent<Animator>().SetTrigger("Bounce");
                }
            }
        }
    }




    [System.Serializable]
    public class TextAnimationInfo {
        public RectTransform rect;
        public Constants.CardStats stat;
    }

}
