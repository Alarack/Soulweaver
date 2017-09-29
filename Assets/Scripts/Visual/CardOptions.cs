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
    public RectTransform unInterceptContainer;
    public RectTransform activateContainer;


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
                break;

            case CardOptionType.Unintercept:
                unInterceptContainer.gameObject.SetActive(show);
                break;
        }
    }

    public void ToggleIntercept() {

        if (myCard.keywords.Contains(Constants.Keywords.Interceptor)) {
            myCard.RPCToggleIntercept(PhotonTargets.All, false);
            interceptContainer.gameObject.SetActive(true);
        }
        else {
            myCard.RPCToggleIntercept(PhotonTargets.All, true);
            unInterceptContainer.gameObject.SetActive(true);
        }

        myCard.owner.combatManager.EndCombat();

        gameObject.SetActive(false);
    }



}
