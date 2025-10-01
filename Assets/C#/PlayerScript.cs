using System.Collections.Generic;
using UnityEngine;

public class PlayerScript
{
    [Header("Setup")]
    public string playerName;
    public int playerID;
    public PlayerScript(string name, int startMana)
    {
        powerCards = new List<CardScript>();
        currentMana = startMana;
    }

    [Header("Mana")]
    public int currentMana;

    [Header("Cards")]
    public List<CardScript> powerCards;
    public CardScript card1;
    public CardScript card2;


    
    public void AddPowerCard(CardScript card) => powerCards.Add(card);
    public void RemovePowerCard(CardScript card) => powerCards.Remove(card);

    public void AddCards(CardScript addcard1, CardScript addcard2)
    {
        card1 = addcard1;
        card2 = addcard2;
    }
}
