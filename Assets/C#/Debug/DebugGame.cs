using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


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

    int firstInput = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (firstInput == 1) SelectNumber(11);
            else firstInput = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (firstInput == 0) SelectNumber(2);
            else SelectNumber(12);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (firstInput == 0) SelectNumber(3);
            else SelectNumber(13);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (firstInput == 0) SelectNumber(4);
            else SelectNumber(14);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectNumber(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SelectNumber(6);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) SelectNumber(7);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) SelectNumber(8);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) SelectNumber(9);
        else if (Input.GetKeyDown(KeyCode.N)) SelectSuit((int)SuitEnum.Nature);
        else if (Input.GetKeyDown(KeyCode.T)) SelectSuit((int)SuitEnum.Time);
        else if (Input.GetKeyDown(KeyCode.E)) SelectSuit((int)SuitEnum.Elemental);
        else if (Input.GetKeyDown(KeyCode.R)) SelectSuit((int)SuitEnum.Rune);

        //else if (Input.GetKeyDown(KeyCode.Esc) && order > 0) DeteleLast();
    }

    void DeleteLast()
    {
        Debug.Log("idk");
    }
    public void SelectSuit(int suit)
    {
        selectedSuit = (SuitEnum)suit;
        selectedSuitText.text = selectedSuit.ToString();
        suitSelected = true;
        AddCard();
        //if(numberSelected) applyButton.interactable = true;
    }
    public void SelectNumber(int number)
    {
        selectedNumber = number;
        selectedNumberText.text = number.ToString();
        numberSelected = true;
        AddCard();
        //if(suitSelected) applyButton.interactable = true;
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
                    this.enabled = false;
                    return;
                }
            }
            order++;
            applyButton.interactable = false;
            selectedNumberText.text = "n";
            selectedSuitText.text = "s";
            numberSelected = false;
            suitSelected = false;
            firstInput = 0;

            if (order == 2) infoText.text = "Add card to player2";
            else if (order >= 4) infoText.text = "Add cards to table: " + (order - 4);
        }
    }

}
