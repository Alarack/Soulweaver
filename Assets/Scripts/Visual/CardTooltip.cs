using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour {

    public static CardTooltip cardTooltip;


    public GameObject tooltipContainer;
    public Text toolTipText;





	void Awake () {
        cardTooltip = this;
	}

	void Update () {
		
	}



    public static void ShowTooltip(string value) {
        if (!cardTooltip.tooltipContainer.activeInHierarchy)
            cardTooltip.tooltipContainer.SetActive(true);

        cardTooltip.toolTipText.text = value;
    }

    public static void HideTooltip() {
        if (cardTooltip.tooltipContainer.activeInHierarchy)
            cardTooltip.tooltipContainer.SetActive(false);

    }
}
