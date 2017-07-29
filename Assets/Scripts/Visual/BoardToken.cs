using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardToken : MonoBehaviour {

    public Image image;
    public Transform incomingEffectLocation;


    protected CardData parentCardData;
    protected CardVisual parentCard;



    public virtual void Initialize(CardData data, CardVisual card) {
        parentCard = card;
        parentCardData = data;

        image.sprite = parentCardData.cardImage;

    }



}
