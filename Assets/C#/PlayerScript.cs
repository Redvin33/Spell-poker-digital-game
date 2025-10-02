using System.Collections.Generic;
using UnityEngine;

public class PlayerScript
{
    [Header("Setup")]
    public string playerName;
    public int playerID;
    public PlayerScript(string name, int startMana, int id)
    {
        powerCards = new List<CardScript>();
        baseCards = new List<Card>();
        currentMana = startMana;
        playerID = id;
    }

    [Header("Mana")]
    public int currentMana;

    [Header("Cards")]
    public List<CardScript> powerCards;
    public List<Card> baseCards;


    //HandChecker
    public HandEnum highestHand;
    public int highestCard;
    
    public void AddPowerCard(CardScript card) => powerCards.Add(card);
    public void RemovePowerCard(CardScript card) => powerCards.Remove(card);
    public void AddCard(Card addedCard)
    {
        baseCards.Add(addedCard);
        Debug.Log("Gave player " + playerID + " card: " + addedCard.cardSuit.ToString() + "/" + addedCard.cardNumber);
    }

    public void AddMana(int amount) => currentMana += amount;
    public bool CheckMana(int required)
    {
        if(currentMana >= required)
        {
            currentMana -= required;
            return true;
        }
        return false;
    }
    public void CheckHigherHand(HandEnum handToCheck)
    {
        if ((int)handToCheck > (int)highestHand) highestHand = handToCheck;
    }
    public void ResetTurn()
    {
        highestHand = HandEnum.None;
        highestCard = 0;
        baseCards.Clear();
    }





}
