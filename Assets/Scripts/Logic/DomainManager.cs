using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainManager : Photon.MonoBehaviour {


    public List<DomainTile> domainTiles = new List<DomainTile>();

    private int domCount;






    #region RPCs

    public void RPCInitializeDomain(PhotonTargets targets) {

        photonView.RPC("InitializeDomain", targets);
    }

    [PunRPC]
    public void InitializeDomain() {
        for(int i = 0; i < domainTiles.Count; i++) {
            domainTiles[i].myDomainCard = GetComponent<Deck>().activeCards[i];

            CardDomainData data = domainTiles[i].myDomainCard.cardData as CardDomainData;

            domainTiles[i].myDomainCard.domainTile = domainTiles[i];

            domainTiles[i].domainImage.sprite = data.domainIcon;
        }
    }

    public void RPCActivateDomainTile(PhotonTargets targets) {
        photonView.RPC("ActivateDomainTile", targets);
    }

    [PunRPC]
    public void ActivateDomainTile() {
        domainTiles[domCount].ActivateTile();
        domCount++;
    }



    #endregion



}
