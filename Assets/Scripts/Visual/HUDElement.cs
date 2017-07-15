using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDElement : Photon.MonoBehaviour {




	void Awake () {
        //RegisterElement();
        photonView.RPC("RegisterElement", PhotonTargets.AllBufferedViaServer);

        if(photonView.isMine)
            photonView.RPC("FixRotation", PhotonTargets.OthersBuffered);
	}

	void Update () {
		
	}




    [PunRPC]
    public void FixRotation() {
        transform.localRotation = Quaternion.Euler(0, 0, 180f);
    }



    [PunRPC]
    public void RegisterElement() {
        HUDRegistrar.AddHudElement(gameObject);
    }
}
