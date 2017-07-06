﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Creature Card")]
public class CardCreatureData : CardData {

    [Header("Creature Stats")]
    public int attack;
    public int size;
    public int health;

}
