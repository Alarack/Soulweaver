using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preserve : MonoBehaviour {

    public static Preserve instance;



	void Awake () {

        DontDestroyOnLoad(gameObject);

        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }


	}
}
