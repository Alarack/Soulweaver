using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameResource {

    public enum ResourceType {
        None = 0,
        Essence,
        Hardlight,
        LostSouls,
    }

    public ResourceType resourceType;
    public string resourceName;
    public int currentValue;
    public int maximumValue;
    public int resourceCap;

    public GameResourceDisplay manager;


    public GameResource(ResourceType type, int currentValue, int maximumValue, string resourceName, GameResourceDisplay manager, int resourceCap = 0) {
        this.resourceType = type;
        this.currentValue = currentValue;
        this.maximumValue = maximumValue;
        this.resourceName = resourceName;
        this.manager = manager;


        //Debug.Log(resourceName + " is a resource");

        if (resourceCap != 0)
            this.resourceCap = resourceCap;
    }



    public void AddResource(int value) {
        currentValue += value;

        if (currentValue > maximumValue && maximumValue > 0)
            currentValue = maximumValue;

        //if (currentValue < maximumValue) {
        //    currentValue += value;
        //}

        UpdateText();
    }

    public bool RemoveResource(int value) {
        if(currentValue - value < 0) {
            return false;
        }
        else {
            currentValue -= value;
            UpdateText();
            return true;
        }
    }

    public void IncreaseMaximum(int value, bool temp = false) {

        if (resourceCap != 0 && maximumValue + value > resourceCap)
            return;

        maximumValue += value;

        UpdateText();
    }

    public void RefreshResource() {
        currentValue = maximumValue;
        UpdateText();
    }

    public void UpdateText() {

        if (maximumValue > 0)
            manager.UpdateResourceText(this, currentValue.ToString() + "/" + maximumValue);
        else
            manager.UpdateResourceText(this, currentValue.ToString());
    }


}