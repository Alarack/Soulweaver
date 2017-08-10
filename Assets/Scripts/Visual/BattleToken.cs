using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleToken : BoardToken {


    //public Image image;
    public Text attack;
    public Text size;
    public Text health;
    [Space(10)]
    public GameObject battleTokenGlow;
    //public Transform incomingEffectLocation;

    private CreatureCardVisual _parentCard;
    private CardCreatureData _creatureData;


    public override void Initialize(CardData data, CardVisual card) {
        base.Initialize(data, card);

        _creatureData = parentCardData as CardCreatureData;
        _parentCard = parentCard as CreatureCardVisual;

        SetUpTokenText();
    }

    public void SetUpTokenText() {
        attack.text = _creatureData.attack.ToString();
        size.text = _creatureData.size.ToString();
        health.text = _creatureData.health.ToString();
    }


    public void UpdateBattleTokenTokenText(Constants.CardStats statToUpdate, int value) {

        //Debug.Log(_parentCard.gameObject.name + " :: " + _parentCard.cardData.cardName + " is updating its battle token");

        switch (statToUpdate) {
            case Constants.CardStats.Attack:
                TextTools.AlterTextColor(value, _creatureData.attack, attack);
                if (value < 0) {
                    attack.text = 0.ToString();
                }
                else {
                    attack.text = value.ToString();
                }
                break;

            case Constants.CardStats.Size:
                TextTools.AlterTextColor(value, _creatureData.size, size);
                size.text = value.ToString();
                break;

            case Constants.CardStats.Health:
                TextTools.AlterTextColor(value, _creatureData.health, health);
                health.text = value.ToString();
                break;

            case Constants.CardStats.MaxHealth:
                TextTools.AlterTextColor(value, _creatureData.health, health);
                health.text = value.ToString();
                break;
        }

    }



}
