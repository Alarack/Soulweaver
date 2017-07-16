﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

public class DomainTile : Photon.MonoBehaviour {

    public CardVisual myDomainCard;
    public Image domainImage;

    public bool active;

	void Start () {



        if (photonView.isMine) {
            RPCRotateTile(PhotonTargets.OthersBuffered);
        }

    }

	void Update () {
		
	}


    public void ActivateDomainAbility() {

        Debug.Log(myDomainCard.cardData.cardName + "'s domain tile has been clicked");

        EventData data = new EventData();
        data.AddMonoBehaviour("Tile", this);
        data.AddMonoBehaviour("Card", myDomainCard);

        Grid.EventManager.SendEvent(Constants.GameEvent.UserActivatedDomainAbility, data);
    }

    public void OnMouseOver() {
        if (myDomainCard != null) {
            Debug.Log(myDomainCard.cardNameText.text + " " + myDomainCard.cardDescriptionText.text);
        }

    }

    public void OnMouseExit() {
        if (myDomainCard != null) {
            Debug.Log("mouse out");
        }

    }

    public void ActivateTile() {
        domainImage.color = Color.blue;
        active = true;
    }







    #region RPCs


    public void RPCRotateTile(PhotonTargets targets) {

        photonView.RPC("RotateTile", targets);
    }

    [PunRPC]
    public void RotateTile() {
        transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
    }

    public void RPCExhaustTile(PhotonTargets targets, bool exhaust) {
        photonView.RPC("ExhaustTile", targets, exhaust);
    }

    [PunRPC]
    public void ExhaustTile(bool exhaust) {
        if (exhaust) {
            domainImage.color = Color.grey;
        }
        else {
            domainImage.color = Color.blue;
        }
    }


    #endregion
}
