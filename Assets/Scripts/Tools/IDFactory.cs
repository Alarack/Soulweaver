using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDFactory  {


    public static int uniqueID;



    public static int GenerateID() {
        uniqueID++;

        return uniqueID;
    }



}
