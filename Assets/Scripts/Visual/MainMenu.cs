using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public RectTransform menuContainer;

    private bool gameEnded;



    public void QuitGame() {
        Application.Quit();
    }


    public void GotoPlayMode() {
        SceneManager.LoadScene("Main");
    }

    public void GoToDeckBuilder() {
        if (gameEnded)
            return;

        if (PhotonNetwork.connected) {

            for (int i = Deck._allCards.activeCards.Count - 1; i >= 0; i--) {
                if (Deck._allCards.activeCards[i] != null)
                    Deck._allCards.activeCards[i].RPCUnregisterCard(PhotonTargets.AllBufferedViaServer, Deck._allCards.activeCards[i].photonView.viewID);
            }

            ClearEventStuff();

            StartCoroutine(EndGame("DeckBuilder"));
            return;
        }

        SceneManager.LoadScene("DeckBuilder");
    }


    public void GoToTitleScene() {

        if (gameEnded)
            return;

        for(int i = Deck._allCards.activeCards.Count -1; i >= 0; i--) {
            if (Deck._allCards.activeCards[i] != null)
                Deck._allCards.activeCards[i].RPCUnregisterCard(PhotonTargets.AllBufferedViaServer, Deck._allCards.activeCards[i].photonView.viewID);
        }

        ClearEventStuff();
        StartCoroutine(EndGame("TitleScreen"));
    }

    private void ClearEventStuff() {
        NetworkManager._allDecks.Clear();
        Mulligan.choosingMulligan = true;

        //Deck._allCards.activeCards.Clear();

        CombatManager cm = FindObjectOfType<CombatManager>();
        if(cm != null)
            Grid.EventManager.RemoveMyListeners(cm);

    }

    public void ShowMenu() {
        if(menuContainer != null) {
            menuContainer.gameObject.SetActive(true);
        }
    }

    private IEnumerator EndGame(string targetScene) {

        gameEnded = true;

        yield return new WaitForSeconds(1.5f);

        if (PhotonNetwork.connected) {
            PhotonNetwork.Disconnect();
        }

        gameEnded = false;
        SceneManager.LoadScene(targetScene);


    }



}
