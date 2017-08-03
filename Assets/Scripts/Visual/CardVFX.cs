using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

public class CardVFX : Photon.MonoBehaviour {

    public float lifetime;
    public float moveSpeed = 1f;
    public float lerpSmoothing = 10f;
    public bool active;

    public bool beginMovement;
    public GameObject impactParticle;
    public Transform target;
    public Text optionalText;
    [Header("Animation Stuff")]
    public Animator animMain;
    public Animator animText;
    public ParticleSystem particles;
    public bool playAnimOnStart;

    private Vector3 position;
    private CardVisual targetCard;

	void Start () {

        //if (!photonView.isMine) {
        //    if (lifetime > 0 && !beginMovement)
        //        Invoke("CleanUp", lifetime);
        //}


        if (playAnimOnStart)
            animMain.SetTrigger("Bounce");

        //active = false;
	}

    public void Initialize(CardVisual target, bool moving, bool sendImpactEvent = true) {

        //if (lifetime > 0 && !moving)
        //    Invoke("CleanUp", lifetime);


        if (target == null)
            return;

        photonView.RPC("RPCInitialize", PhotonTargets.All, target.photonView.viewID, moving);


        if (moving) {
            StartCoroutine(StartMovement());
        }
        else {
            //RPCSendImpactEvent(PhotonTargets.Others);
            //StartCoroutine(SendImmediateEvent());
            if(sendImpactEvent)
                SendImmediateEvent();
        }

        //StartCoroutine(StartMovement());



        //RPCInitialize(targetCard.photonView.viewID, moving);

    }

	void Update () {

        if(beginMovement && target != null) {

            if (photonView.isMine) {
                if(MoveTowardsTarget(target, 0.2f)) {
                    if(impactParticle != null) {
                        GameObject impact = PhotonNetwork.Instantiate(impactParticle.name, target.position, Quaternion.identity, 0) as GameObject;
                        CardVFX impactVFX = impact.GetComponent<CardVFX>();
                        impactVFX.Initialize(targetCard, false, false);
                        //impactVFX.photonView.RPC("RPCInitialize", PhotonTargets.Others, 0);
                        RPCSendImpactEvent(PhotonTargets.Others);
                        SendImpactEvent();
                        Invoke("NetworkCleanup", 0.3f);
                        //Destroy(gameObject, 0.5f);
                    }
                }
            }
        }

        if (!photonView.isMine) {
            if (active) {
                //transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
                transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed);
            }
        }
    }//End of Update





    public void SetText(string textValue) {

        if(optionalText != null) {
            optionalText.gameObject.SetActive(true);

            optionalText.text = textValue;
        }
    }

    public void PlayAnim() {
        animMain.SetTrigger("Bounce");
        animText.SetTrigger("Fade");
    }

    public void PlayParticles() {
        particles.Play();
    }


    public IEnumerator StartMovement() {
        yield return new WaitForSeconds(0.7f);

        beginMovement = true;


    }

    public void SendImmediateEvent() {
        //yield return new WaitForSeconds(0.2f);
        //Debug.Log(gameObject.name + " is Sending VFX land Event");

        RPCSendImpactEvent(PhotonTargets.Others);
        SendImpactEvent();
    }


    public bool MoveTowardsTarget(Transform target, float minDistance) {

        if (Vector3.Distance(transform.position, target.transform.position) > minDistance){
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed);
            return false;
        }
        else {
            beginMovement = false;
            return true;
        }



    }



    public void CleanUp() {
        Destroy(gameObject);
    }

    public void NetworkCleanup() {
        PhotonNetwork.Destroy(gameObject);
    }



    #region RPCs

    public void RPCSetVFXAciveState(PhotonTargets targets, bool activate) {
        photonView.RPC("SetVFXActiveState", targets, activate);
    }

    [PunRPC]
    public void SetVFXActiveState(bool activate) {
        active = activate;
    }

    [PunRPC]
    public void RPCInitialize(int cardID, bool moving) {
        CardVisual target = Finder.FindCardByID(cardID);

        if (target == null)
            return;


        if (lifetime > 0 && !moving)
            Invoke("CleanUp", lifetime);

        this.targetCard = target;

        if (target is CreatureCardVisual) {
            CreatureCardVisual soul = target as CreatureCardVisual;
            this.target = soul.battleToken.incomingEffectLocation;
        }
    }

    public void RPCSendImpactEvent(PhotonTargets targets) {
        photonView.RPC("SendImpactEvent", targets);
    }

    [PunRPC]
    public void SendImpactEvent() {
        EventData data = new EventData();
        data.AddMonoBehaviour("Card", targetCard);
        data.AddMonoBehaviour("VFX", this);

        Grid.EventManager.SendEvent(Constants.GameEvent.VFXLanded, data);

    }


    #endregion





    #region PhotonNetwork Methods

    protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.position);
        }
        else {
            position = (Vector3)stream.ReceiveNext();
        }
    }

    #endregion
}
