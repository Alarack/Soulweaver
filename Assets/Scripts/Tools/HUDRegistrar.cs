using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDRegistrar : MonoBehaviour {


    public static HUDRegistrar hudRegistrar;

    public List<GameObject> hudElements;


	void Awake () {
        hudRegistrar = this;
	}


    public static void AddHudElement(GameObject element) {

        hudRegistrar.hudElements.Add(element);

    }

    public static void RemoveHudElement(GameObject element) {

        if(hudRegistrar.hudElements.Contains(element))
            hudRegistrar.hudElements.Remove(element);

    }


    public static GameObject FindHudElementByID(int id) {

        for(int i = 0; i < hudRegistrar.hudElements.Count; i++) {
            if (hudRegistrar.hudElements[i].GetPhotonView().viewID == id)
                return hudRegistrar.hudElements[i];
        }

        return null;
    }


}
