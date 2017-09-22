using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public RectTransform menuContainer;



    public void GotoPlayMode() {
        SceneManager.LoadScene("Main");
    }

    public void GoToDeckBuilder() {


        if (PhotonNetwork.connected) {
            for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {

                if (Deck._allCards.activeCards[i] == null)
                    continue;

                Deck._allCards.activeCards[i].UnregisterEverything();
            }

            ClearEventStuff();



            PhotonNetwork.Disconnect();
        }

        SceneManager.LoadScene("DeckBuilder");
    }


    public void GoToTitleScene() {
        for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            //Debug.Log(Deck._allCards.activeCards[i].gameObject.name + " is unreging");

            if (Deck._allCards.activeCards[i] == null)
                continue;

            Deck._allCards.activeCards[i].UnregisterEverything();
        }

        ClearEventStuff();

        if (PhotonNetwork.connected) {
            PhotonNetwork.Disconnect();
        }

        SceneManager.LoadScene("TitleScreen");

    }

    private void ClearEventStuff() {
        NetworkManager._allDecks.Clear();
        Mulligan.choosingMulligan = true;

        Deck._allCards.activeCards.Clear();

        CombatManager cm = FindObjectOfType<CombatManager>();
        if(cm != null)
            Grid.EventManager.RemoveMyListeners(cm);

    }

    public void ShowMenu() {
        if(menuContainer != null) {
            menuContainer.gameObject.SetActive(true);
        }

    }



}
