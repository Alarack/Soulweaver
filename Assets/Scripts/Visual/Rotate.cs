using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    [Header("Basic Attributes")]
    public float zAxisRotateSpeed;
    public float yAxisRotateSpeed;
    public float xAxisRotateSpeed;
    [Header("Options")]
    public bool beginRotationAfterDelay;
    public float initialDelay;
    [Space]
    public bool flipRotationDirection;
    public float flipInterval;
    [Space(10)]
    public bool alterSpeedAfterDelay;
    public float alterSpeedDelay;
    public float alterSpeedModifier;


    private float flipTimer;
    private float alterSpeedTimer;

    void Start() {


    }

    void Update() {

        if (!beginRotationAfterDelay)
            BeginRotation();
        else {
            Invoke("ToggleRotation", initialDelay);
        }


    }


    private void BeginRotation() {
        //Debug.Log("Rotating");

        transform.Rotate(xAxisRotateSpeed * Time.deltaTime, yAxisRotateSpeed * Time.deltaTime, zAxisRotateSpeed * Time.deltaTime);

        if (flipRotationDirection) {
            FlipRotation();
        }

        if (alterSpeedAfterDelay && alterSpeedTimer < alterSpeedDelay) {
            AlterSpeed();
        }

    }



    private void AlterSpeed() {
        alterSpeedTimer += Time.deltaTime;

        if (alterSpeedTimer >= alterSpeedDelay) {
            zAxisRotateSpeed *= alterSpeedModifier;
            yAxisRotateSpeed *= alterSpeedModifier;
            xAxisRotateSpeed *= alterSpeedModifier;
        }
    }

    private void FlipRotation() {

        flipTimer += Time.deltaTime;

        if (flipTimer >= flipInterval) {
            zAxisRotateSpeed *= -1;
            yAxisRotateSpeed *= -1;
            xAxisRotateSpeed *= -1;

            flipTimer = 0;
        }

    }

    private void ToggleRotation() {
        beginRotationAfterDelay = false;
    }


}
