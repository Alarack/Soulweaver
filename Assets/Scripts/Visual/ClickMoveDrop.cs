using UnityEngine;
using System.Collections;

public class ClickMoveDrop : MonoBehaviour {


    public bool moveableIsGrabbed = false;
    public Transform moveableTransform;
    public LayerMask whatIsMoveableObject;
    public LayerMask whatIsGround;
    public float moZOffset = 0f;
    //public float mousePositionYOffset = 0f;
    private Vector3 mousePositionRelativeToGround;

    private Ray moRay;
    private RaycastHit moHit;

    private RaycastHit mousePosHit;
    //public TargetUtils targetUtils;


    void Start() {
        moHit = new RaycastHit();
        //targetUtils = GetComponent<TargetUtils>();


    }

    void Update() {
        if (Camera.main != null)
            moRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        else {
            Debug.LogError("[CLick Move Drop] Camera is Null");
        }


        if (Input.GetMouseButtonDown(0) /*&& !targetUtils.isChoosingTarget && !targetUtils.isInCombat*/) {
            FindAndGrab();
        }

        if (Input.GetMouseButtonUp(0)) {
            DropObject();
        }

        if (moveableTransform != null)
            moveableIsGrabbed = true;
        else
            moveableIsGrabbed = false;

        if (moveableIsGrabbed)
            TraceMousePosition();

    }

    void FindAndGrab() {

        if (Physics.Raycast(moRay, out moHit, Mathf.Infinity, whatIsMoveableObject)) {

            Transform target = moHit.collider.gameObject.transform;
            //Debug.Log(target.gameObject.name);

            if (target.gameObject.GetPhotonView().isMine && target.gameObject.GetComponent<CardVisual>() != null &&
               target.gameObject.GetComponent<CardVisual>().currentDeck.decktype == Constants.DeckType.Hand /*&& !Mulligan.choosingMulligan*/) {

                //Debug.Log("Clicked on a card");
                moveableTransform = target;
            }
        }
    }

    void TraceMousePosition() {
        if (Physics.Raycast(moRay, out mousePosHit, Mathf.Infinity, whatIsGround)) {
            mousePositionRelativeToGround = mousePosHit.point;
            moveableTransform.position = new Vector3(mousePositionRelativeToGround.x, mousePositionRelativeToGround.y, moveableTransform.position.z);

        }
    }

    void DropObject() {
        if (moveableTransform != null) {
            moveableTransform = null;
        }
    }
}