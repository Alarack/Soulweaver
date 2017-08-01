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

        if(lifetime > 0)
            Invoke("CleanUp", lifetime);

        if (playAnimOnStart)
            animMain.SetTrigger("Bounce");

        //active = false;
	}

    public void Initialize(CardVisual target, bool moving) {

        photonView.RPC("RPCInitialize", PhotonTargets.All, target.photonView.viewID);


        if (moving) {
            StartCoroutine(StartMovement());
        }
        else {
            RPCSendImpactEvent(PhotonTargets.Others);
            SendImpactEvent();
        }

        //StartCoroutine(StartMovement());

        

        //RPCInitialize(targetCard.photonView.viewID, moving);

    }

	void Update () {


        if(beginMovement && target != null) {

            if (photonView.isMine) {
                if(MoveTowardsTarget(target, 0.2f)) {
                    if(impactParticle != null) {
                        PhotonNetwork.Instantiate(impactParticle.name, target.position, Quaternion.identity, 0);


                        RPCSendImpactEvent(PhotonTargets.Others);
                        SendImpactEvent();
                        Invoke("NetworkCleanup", 0.3f);
                        //Destroy(gameObject, 0.5f);
                    }
                        

                }
            }

        }




        if (!photonView.isMine) {

            //Debug.Log(position);

            if (active) {
                //transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
                transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed);
            }
        }

    }





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
    public void RPCInitialize(int cardID) {
        CardVisual target = Finder.FindCardByID(cardID);

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
