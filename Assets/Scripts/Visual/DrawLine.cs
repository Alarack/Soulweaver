using UnityEngine;

public class DrawLine : Photon.MonoBehaviour {


    public LineRenderer lineRenderer;


    private Vector3 _initialPosition;
    private Vector3 _currentPosition;
    private Vector3 _mousePosition;
    private Vector3 _targetPosition;

    public void Start() {
        lineRenderer.enabled = false;
    }




    public void BeginDrawing(Vector3 source, Vector3 mousePos) {
        _initialPosition = source;
        _mousePosition = Input.mousePosition;

        _currentPosition = GetCurrentMousePosition(_mousePosition).GetValueOrDefault();

        lineRenderer.SetPosition(0, _initialPosition);

        lineRenderer.numPositions = 2;
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(1, _currentPosition);
    }



    public void RPCBeginDrawing(PhotonTargets targets, Vector3 sourcePos, Vector3 targetPos) {
        float xpos = sourcePos.x;
        float ypos = sourcePos.y;
        float zpos = sourcePos.z;

        float targetX = targetPos.x;
        float targetY = targetPos.y;
        float targetZ = targetPos.z;

        photonView.RPC("BeginDrawing", targets, xpos, ypos, zpos, targetX, targetY, targetZ);

    }


    [PunRPC]
    public void BeginDrawing(float xpos, float ypos, float zpos, float mouseX, float mouseY, float mouseZ) {
        _initialPosition = new Vector3(xpos, ypos, zpos);
        _targetPosition = new Vector3(mouseX, mouseY, mouseZ);

        //_currentPosition = GetCurrentMousePosition(_mousePosition).GetValueOrDefault();

        lineRenderer.SetPosition(0, _initialPosition);

        lineRenderer.numPositions = 2;
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(1, _targetPosition);

    }


    public void RPCStopDrawing(PhotonTargets targets) {
        photonView.RPC("StopDrawing", targets);
    }

    [PunRPC]
    public void StopDrawing() {
        lineRenderer.enabled = false;

    }




    private Vector3? GetCurrentMousePosition(Vector3 mousePos) {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        var plane = new Plane(Vector3.forward, new Vector3(0,0, -15f));

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance)) {
            return ray.GetPoint(rayDistance);

        }

        return null;
    }

}