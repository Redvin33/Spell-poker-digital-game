using NUnit.Framework; //?
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("Deck")]
    public List<Card> baseCards;
    public int cardsPerSuit;
    public int suitsAmount;

    [Header("Special Deck")]
    public List<Card> specialCards = new List<Card>();

    [Header("Players")]
    public List<PlayerScript> playerScripts;
    public int testStartMana;
    public int testPlayerCount;
    public int handCardAmount;

    [Header("DEBUGLISTPLAYERCARDS")]
    [SerializeField] public List<String> player1Cards = new List<String>();
    [SerializeField] public List<String> player2Cards = new List<String>();
    [SerializeField] public List<String> tableCards = new List<String>();

    [Header("Current cards on table")]
    public List<Card> cardsOnTable;


    private void Awake()
    {
        //TESTING
        cardsOnTable = new List<Card>();
        CreateBaseDeck(cardsPerSuit, suitsAmount);
    }
    public void CreateBaseDeck(int cardsPerSuit, int suitsAmount) //Implement A, 2-14 or 1-13??
    {
        baseCards = new List<Card>();
        for(int i = 0; i < suitsAmount; i++)
        {
            for(int j = 0; j < cardsPerSuit; j++)
            {
                baseCards.Add(new Card((SuitEnum)i, j + 1, null)); //j = 0 on start, +1 to make the first card 1 (==A?)
                //Each card gets func, send suit + num, return art accordingly.
            }
        }
        Debug.Log("Created Base deck, cardAmount: " + baseCards.Count);
        Debug.Log("---------------------------------------------------------------");
        //base 52 baseCards created.

        //TESTING
        CreatePlayers(testPlayerCount, testStartMana);
    }

    public void CreatePlayers(int playerCount, int startMana)
    {
        playerScripts = new List<PlayerScript>();

        for(int i = 0; i < playerCount; i++)
        {
            playerScripts.Add(new PlayerScript("Player " + (i + 1), startMana, i+1));

            Debug.Log("Created Player " + (i+1));
        }
        Debug.Log("---------------------------------------------------------------");


        //TESTING
        RandomizeCardLists();
    }

    public void RandomizeCardLists()
    {
        ListRandomizer.Shuffle(baseCards);
        Debug.Log("Cards Randomized:");
        for (int i = 0; i < baseCards.Count; i++)
        {
            Debug.Log("Card " + i + ". Suit: " + baseCards[i].cardSuit.ToString() + ", Number: " + baseCards[i].cardNumber);
        }
        Debug.Log("---------------------------------------------------------------");
        //TESTING
        DealCardsToPlayers();
    }
    public void DealCardsToPlayers()
    {
        int cardsToDeal = playerScripts.Count * handCardAmount;
        int startIndex = 0;

        for(int i = 0; i < cardsToDeal;i++)
        {
            var cardPicked = baseCards[0];
            baseCards.Remove(cardPicked);
            playerScripts[startIndex].AddCard(cardPicked);

            if(startIndex == 0) player1Cards.Add(cardPicked.cardSuit.ToString()+"/"+cardPicked.cardNumber); //DEBUG
            else player2Cards.Add(cardPicked.cardSuit.ToString() + "/" + cardPicked.cardNumber);//DEBUG


            startIndex++;
            if (startIndex > playerScripts.Count - 1) startIndex = 0;
        }
        Debug.Log("---------------------------------------------------------------");
        PhaseTester();
    }

    public void PhaseTester() //? Add 3 cards to table. Change phases later accordingly to add blinds, bets etc.
    {
        cardsOnTable.Clear();

        for (int i = 0; i < 3; i++) AddCardOnTable();
        Invoke(nameof(AddCardOnTable), 2f);
        Invoke(nameof(AddCardOnTable), 4f);
    }

    public void AddCardOnTable()
    {
        var cardToAdd = baseCards[0];
        cardsOnTable.Add(cardToAdd);
        Debug.Log("Card " + (cardsOnTable.Count) + ":" + cardToAdd.cardSuit.ToString() + "/" + cardToAdd.cardNumber);
        tableCards.Add(cardToAdd.cardSuit.ToString() + "/" + cardToAdd.cardNumber); //DEBUG
        baseCards.Remove(cardToAdd);

        if (cardsOnTable.Count == 5) DetermineWinner();
    }

    public void DetermineWinner()
    {

    }

    public void ResetCards(Card card) //return and reset cards, or create a whole new deck? new is less garbage?
    {
        card.ResetCard();
        baseCards.Add(card);
    }
}
