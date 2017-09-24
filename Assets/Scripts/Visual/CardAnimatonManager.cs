using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAnimatonManager : MonoBehaviour {


    public List<TextAnimationInfo> textAnimInfo = new List<TextAnimationInfo>();
    [Space(15)]
    public List<KeywordVisualInfo> keywordVisualInfo = new List<KeywordVisualInfo>();
    public List<SpecialAttributeVisualInfo> specialAttributeVisualInfo = new List<SpecialAttributeVisualInfo>();

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

    public void ShowOrHideKeywordVisual(Constants.Keywords keyword, bool add) {

        for(int i = 0; i < keywordVisualInfo.Count; i++) {
            if (keywordVisualInfo[i].keyword == keyword) {
                keywordVisualInfo[i].ShowOrHide(add);
                break;
            }
        }
    }

    public void ShowOrHideSpecialAttributeInfo(SpecialAttribute.AttributeType attributeType, bool add) {
        for (int i = 0; i < specialAttributeVisualInfo.Count; i++) {
            if (specialAttributeVisualInfo[i].attributeType == attributeType) {
                specialAttributeVisualInfo[i].ShowOrHide(add);
                break;
            }
        }
    }



    [System.Serializable]
    public class TextAnimationInfo {
        public RectTransform rect;
        public Constants.CardStats stat;
    }

    [System.Serializable]
    public class KeywordVisualInfo {
        public Constants.Keywords keyword;
        public GameObject visualEffect;


        public void ShowOrHide(bool show) {
            if (show)
                Show();
            else
                Hide();
        }

        public void Show() {
            if(visualEffect != null && !visualEffect.activeInHierarchy) {
                visualEffect.SetActive(true);
            }
        }

        public void Hide() {
            if (visualEffect != null && visualEffect.activeInHierarchy) {
                visualEffect.SetActive(false);
            }
        }

    }

    [System.Serializable]
    public class SpecialAttributeVisualInfo : KeywordVisualInfo {

        public SpecialAttribute.AttributeType attributeType;

    }

}
