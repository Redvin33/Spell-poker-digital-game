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


    
    public void AddPowerCard(CardScript card) => powerCards.Add(card);
    public void RemovePowerCard(CardScript card) => powerCards.Remove(card);
    public void AddCard(Card addedCard)
    {
        baseCards.Add(addedCard);
        Debug.Log("Gave player " + playerID + " card: " + addedCard.cardSuit.ToString() + "/" + addedCard.cardNumber);
    }
}
