using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeywordReminderManager : MonoBehaviour {

    public RectTransform reminderContainer;
    public KeywordReminder reminderTemplate;

    public List<KeywordReminder> reminderList = new List<KeywordReminder>();

    private CardVisual myCard;

    private void Start() {
        myCard = GetComponentInParent<CardVisual>();
    }


    public void CreateReminders(List<Constants.Keywords> keywords) {
        ClearAllReminders();

        if (myCard != null) {
            if (myCard.transform.position.x > Screen.width / 2) {
                reminderContainer.localPosition = new Vector2(-110f, reminderContainer.localPosition.y);
            }
            else {
                reminderContainer.localPosition = new Vector2(100f, reminderContainer.localPosition.y);
            }
        }



        for(int i = 0; i < keywords.Count; i++) {
            GameObject reminder = Instantiate(reminderTemplate.gameObject) as GameObject;
            reminder.transform.SetParent(reminderContainer, false);
            reminder.SetActive(true);

            KeywordReminder reminderScript = reminder.GetComponent<KeywordReminder>();
            reminderScript.Initialize(keywords[i]);

            reminderList.Add(reminderScript);
        }


    }

    public void ClearAllReminders() {
        for (int i = 0; i < reminderList.Count; i++) {
            if(reminderList[i] != null)
                Destroy(reminderList[i].gameObject);
        }

    }



}
