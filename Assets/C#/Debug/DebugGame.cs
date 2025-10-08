using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugGame : MonoBehaviour
{
    public GameObject addCardsPanel;
    public TMP_Text infoText;
    public GameStateManager stateManager;

    public TMP_Text selectedNumberText;
    public TMP_Text selectedSuitText;
    public SuitEnum selectedSuit;
    public int selectedNumber;

    public Button applyButton;

    bool numberSelected;
    bool suitSelected;

    int order;
    public void StartDebugGame()
    {
        order = 0;
        addCardsPanel.SetActive(true);
        infoText.text = "Add card to player1";

        selectedNumberText.text = "n";
        selectedSuitText.text = "s";
        numberSelected = false;
        suitSelected = false;
        applyButton.interactable = false;
        tableOrder = 0;
    }

    public void SelectSuit(int suit)
    {
        selectedSuit = (SuitEnum)suit;
        selectedSuitText.text = selectedSuit.ToString();
        suitSelected = true;
        if(numberSelected) applyButton.interactable = true;
    }
    public void SelectNumber(int number)
    {
        selectedNumber = number;
        selectedNumberText.text = number.ToString();
        numberSelected = true;
        if(suitSelected) applyButton.interactable = true;
    }

    int tableOrder;
    public void AddCard()
    {
        if(suitSelected && numberSelected)
        {
            Card chosen = new Card(selectedSuit, selectedNumber, null);
            if (order < 2)
            {

                stateManager.playerScripts[0].baseCards.Add(chosen);
                stateManager.ui.playerUIDebug[0].AssignCards(chosen);
            }
            else if (order < 4)
            {
                stateManager.playerScripts[1].baseCards.Add(chosen);
                stateManager.ui.playerUIDebug[1].AssignCards(chosen);
            }
            else
            {
                stateManager.cardsOnTable.Add(chosen);
                stateManager.tableCardScripts[tableOrder].DealCard(chosen.cardNumber, chosen.cardSuit);
                tableOrder++;
                if (order == 8)
                {
                    addCardsPanel.SetActive(false);
                    stateManager.DetermineWinner();
                    return;
                }
            }
            order++;
            applyButton.interactable = false;
            selectedNumberText.text = "n";
            selectedSuitText.text = "s";
            numberSelected = false;
            suitSelected = false;

            if (order == 2) infoText.text = "Add card to player2";
            else if (order >= 4) infoText.text = "Add cards to table: " + (order - 4);
        }
    }

}
