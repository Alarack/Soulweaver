using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardOptions : MonoBehaviour {

    public enum CardOptionType {
        Intercept,
        Unintercept,
        Activate
    }



    public RectTransform interceptContainer;
    //public RectTransform unInterceptContainer;
    public RectTransform activateContainer;

    public Sprite interceptSprite;
    public Sprite unInterceptSprite;

    private CardVisual myCard;

    void Start() {
        myCard = GetComponentInParent<CardVisual>();
    }



    public void ShowOrHideElement(CardOptionType type, bool show) {

        switch (type) {
            case CardOptionType.Activate:
                activateContainer.gameObject.SetActive(show);
                
                break;

            case CardOptionType.Intercept:
                interceptContainer.gameObject.SetActive(show);
                interceptContainer.GetComponent<Image>().sprite = interceptSprite;
                break;

            case CardOptionType.Unintercept:
                interceptContainer.gameObject.SetActive(show);
                interceptContainer.GetComponent<Image>().sprite = unInterceptSprite;
                break;
        }
    }

    public void ToggleIntercept() {

        if (myCard.keywords.Contains(Constants.Keywords.Interceptor)) {
            myCard.RPCToggleIntercept(PhotonTargets.All, false);
            interceptContainer.gameObject.SetActive(true);
            interceptContainer.GetComponent<Image>().sprite = interceptSprite;
        }
        else {
            myCard.RPCToggleIntercept(PhotonTargets.All, true);
            //unInterceptContainer.gameObject.SetActive(true);
            interceptContainer.GetComponent<Image>().sprite = unInterceptSprite;
        }

        myCard.owner.combatManager.EndCombat();

        gameObject.SetActive(false);
    }

    public void ActivateAbility() {
        myCard.owner.combatManager.EndCombat();

        if (myCard.CheckForUserActivatedAbilities()) {
            myCard.ActivateAbility();
        }

        gameObject.SetActive(false);

    }



}
