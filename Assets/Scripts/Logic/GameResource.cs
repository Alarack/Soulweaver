using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameResource {

    public enum ResourceType {
        Essence,
        Hardlight,
        LostSouls,
    }

    public ResourceType resourceType;
    public int currentValue;
    public int maximumValue;

    public GameResourceDisplay manager;


    public GameResource(ResourceType type, int currentValue, int maximumValue, GameResourceDisplay manager) {
        this.resourceType = type;
        this.currentValue = currentValue;
        this.maximumValue = maximumValue;
        this.manager = manager;
    }



    public void AddResource(int value) {
        currentValue += value;

        if (currentValue > maximumValue)
            currentValue = maximumValue;

        if (currentValue < maximumValue) {
            currentValue += value;
        }

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
        maximumValue += value;
        UpdateText();
    }

    private void UpdateText() {
        manager.UpdateResourceText(this, currentValue.ToString() + "/" + maximumValue);
    }


}