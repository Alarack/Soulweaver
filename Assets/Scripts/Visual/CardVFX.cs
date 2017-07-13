﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardVFX : Photon.MonoBehaviour {

    public float lifetime;
    public float moveSpeed = 1f;
    public float lerpSmoothing = 10f;
    public bool active;

    public bool beginMovement;
    public Transform target;
    public Text optionalText;
    [Header("Animation Stuff")]
    public Animator animMain;
    public Animator animText;
    public ParticleSystem particles;
    public bool playAnimOnStart;

    private Vector3 position;

	void Start () {

        if(lifetime > 0)
            Invoke("CleanUp", lifetime);

        if (playAnimOnStart)
            animMain.SetTrigger("Bounce");

        active = false;
	}

	void Update () {


        if(beginMovement && target != null) {
            MoveTowardsTarget(target, 0.2f);
        }




        if (!photonView.isMine) {
            if (active) {
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
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


    public bool MoveTowardsTarget(Transform target, float minDistance) {

        if(Vector3.Distance(transform.position, target.transform.position) < minDistance){
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed);
            return false;
        }
        else {
            return true;
        }



    }



    public void CleanUp() {
        Destroy(gameObject);
    }



    #region RPCs

    public void RPCSetVFXAciveState(PhotonTargets targets, bool activate) {
        photonView.RPC("SetCardActiveState", targets, activate);
    }

    [PunRPC]
    public void SetVFXActiveState(bool activate) {
        active = activate;
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