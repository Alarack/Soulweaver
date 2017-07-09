using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleToken : MonoBehaviour {


    public Image image;
    public Text attack;
    public Text size;
    public Text health;
    [Space(10)]
    public GameObject battleTokenGlow;

    private CreatureCardVisual _parentCard;
    private CardCreatureData _creatureData;

    public void Initalize(CardCreatureData data, CreatureCardVisual parentCard) {
        _creatureData = data;

        _parentCard = parentCard;
        image.sprite = data.cardImage;
        SetUpTokenText();
    }

    public void SetUpTokenText() {
        attack.text = _creatureData.attack.ToString();
        size.text = _creatureData.size.ToString();
        health.text = _creatureData.health.ToString();
    }


    public void UpdateBattleTokenTokenText(Constants.CardStats statToUpdate, int value) {
        switch (statToUpdate) {
            case Constants.CardStats.Attack:
                TextTools.AlterTextColor(value, _creatureData.attack, attack);
                attack.text = value.ToString();
                break;

            case Constants.CardStats.Size:
                TextTools.AlterTextColor(value, _creatureData.size, size);
                size.text = value.ToString();
                break;

            case Constants.CardStats.Health:
                TextTools.AlterTextColor(value, _creatureData.health, health);
                health.text = value.ToString();
                break;
        }

    }



}
