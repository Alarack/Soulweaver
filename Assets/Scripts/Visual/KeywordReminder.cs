using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeywordReminder : MonoBehaviour {

    public Text header;
    public Text body;

    public Constants.Keywords keyword;


    public void Initialize(Constants.Keywords keyword) {
        header.text = keyword.ToString();
        this.keyword = keyword;

        SetKeywordReminderText();
    }

    public void SetKeywordReminderText() {

        switch (keyword) {
            case Constants.Keywords.Inspire:
                body.text = "Does something when you play this from your hand.";
                break;

            case Constants.Keywords.Finale:
                body.text = "Does something when this dies.";
                break;

            case Constants.Keywords.Protection:
                body.text = "Ignore an amount of damage from all sources.";
                break;

            case Constants.Keywords.Volatile:
                body.text = "Deal an amount of damage to the source that kills this.";
                break;

            case Constants.Keywords.FirstStrike:
                body.text = "Deals damage first in combat. If the enemy dies, this takes no damage.";
                break;

            case Constants.Keywords.Flight:
                body.text = "Cannot be attacked, blocked, or block Souls without flight.";
                break;

            case Constants.Keywords.Fusion:
                body.text = "Fuses with all other friendly souls with Fusion a the start of your turn.";
                break;

            case Constants.Keywords.Cleave:
                body.text = "Deals damage to adjacent targets when attacking.";
                break;

            case Constants.Keywords.Defender:
                body.text = "Cannot attack";
                break;

            case Constants.Keywords.Pacifist:
                body.text = "Cannot attack or block";
                break;

            case Constants.Keywords.Invisible:
                body.text = "Cannot be attacked, be blocked, or block.";
                break;

            case Constants.Keywords.Phalanx:
                body.text = "Grants a bonus to friendly adjacent souls.";
                break;

            case Constants.Keywords.Ranged:
                body.text = "Cannot counter-attack, or be counter-attacked";
                break;

            case Constants.Keywords.Reach:
                body.text = "Can block souls with Flight.";
                break;

            case Constants.Keywords.Reanimator:
                body.text = "Returns to your hand if this has been dead for an amount of turns.";
                break;

            case Constants.Keywords.Regeneration:
                body.text = "Restores an amount of each at the begining of each of your turns.";
                break;

            case Constants.Keywords.Tireless:
                body.text = "This does not become exhausted when it attacks.";
                break;

            case Constants.Keywords.Token:
                body.text = "This card was created by another card.";
                break;

            case Constants.Keywords.Rush:
                body.text = "Can attack immediately, but not Generals.";
                break;

            case Constants.Keywords.Interceptor:
                body.text = "THis is currently blocking. It must be attacked before non Interceptors";
                break;




        }


    }





}