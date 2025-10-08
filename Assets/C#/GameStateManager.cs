using NUnit.Framework; //?
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("Setup")]
    public UI ui;

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

    [SerializeField] public bool debugDeckBuilding;
    [SerializeField] public bool debugCardDealing;
    [SerializeField] public bool debugTableCards;


    [Header("Current cards on table")]
    public List<Card> cardsOnTable;


    [Header("Table Card Scripts")]
    [SerializeField] public TableCardScript[] tableCardScripts;

    private void Awake()
    {
        //TESTING
        cardsOnTable = new List<Card>();

        //CreateBaseDeck(cardsPerSuit, suitsAmount);
        CreatePlayers(testPlayerCount,testStartMana, false);
        StartGame();
    }

    public void StartGame()
    {
        cardsOnTable.Clear();
        tableCards.Clear(); //DEBUG
        ui.EnableNextRoundButton(false);
        ui.EnableStartButtons(true);
    }
    public void DefaultStart()
    {
        CreateBaseDeck(cardsPerSuit, suitsAmount);
    }
    #region DEBUG

    [Header("Game Debug")]
    public DebugGame debugGame;
    public void DebugStart()
    {
        CreateBaseDeckDebug(cardsPerSuit, suitsAmount);
    }
    
    public void CreateBaseDeckDebug(int cardsPerSuit, int suitsAmount) //Implement A, 2-14 or 1-13??
    {
        Debug.Log("??");
        baseCards = new List<Card>();
        for (int i = 0; i < suitsAmount; i++)
        {
            for (int j = 0; j < cardsPerSuit; j++)
            {
                baseCards.Add(new Card((SuitEnum)i, j + 2, null));
            }
        }
        debugGame.StartDebugGame();
    }
    #endregion
    public void CreateBaseDeck(int cardsPerSuit, int suitsAmount) //Implement A, 2-14 or 1-13??
    {
        baseCards = new List<Card>();
        for(int i = 0; i < suitsAmount; i++)
        {
            for(int j = 0; j < cardsPerSuit; j++)
            {
                baseCards.Add(new Card((SuitEnum)i, j + 2, null)); //j = 0 on start, +2 to make the first card 2. 2-14.
                //Each card gets func, send suit + num, return art accordingly.
            }
        }
        if (debugDeckBuilding) Debug.Log("Created Base deck, cardAmount: " + baseCards.Count);
        if (debugDeckBuilding) Debug.Log("---------------------------------------------------------------");
        //base 52 baseCards created.

        //TESTING
        //CreatePlayers(testPlayerCount, testStartMana, true);
        RandomizeCardLists();
    }

    public void CreatePlayers(int playerCount, int startMana, bool Continue) //CONTINUE INCLUDED TO ALLOW DEBUG&RANDOMIZED START!
    {
        playerScripts = new List<PlayerScript>();

        for(int i = 0; i < playerCount; i++)
        {
            playerScripts.Add(new PlayerScript("Player " + (i + 1), startMana, i+1));
            ui.playerUIDebug[i].AssignPlayer(playerScripts[i].playerName, playerScripts[i].currentMana);

            if (debugDeckBuilding) Debug.Log("Created Player " + (i + 1));
        }
        if (debugDeckBuilding) Debug.Log("---------------------------------------------------------------");


        //TESTING
        if(Continue) RandomizeCardLists();
    }

    public void RandomizeCardLists()
    {
        ListRandomizer.Shuffle(baseCards);

        if (debugDeckBuilding)//DEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUG
        {
            Debug.Log("Cards Randomized:");
            for (int i = 0; i < baseCards.Count; i++)
            {
                Debug.Log("Card " + i + ". Suit: " + baseCards[i].cardSuit.ToString() + ", Number: " + baseCards[i].cardNumber);
            }
            Debug.Log("---------------------------------------------------------------");
        }

        //TESTING
        DealCardsToPlayers();
    }
    public void DealCardsToPlayers()
    {
        Debug.Log("DEaling carsd to plauyers;");
        int cardsToDeal = playerScripts.Count * handCardAmount;
        int startIndex = 0;

        for(int i = 0; i < cardsToDeal;i++)
        {
            var cardPicked = baseCards[0];
            baseCards.Remove(cardPicked);
            playerScripts[startIndex].AddCard(cardPicked);
            ui.playerUIDebug[startIndex].AssignCards(cardPicked);

            if(debugCardDealing)//DEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUG
            {
                if (startIndex == 0) player1Cards.Add(cardPicked.cardSuit.ToString() + "/" + cardPicked.cardNumber); //DEBUG
                else player2Cards.Add(cardPicked.cardSuit.ToString() + "/" + cardPicked.cardNumber);//DEBUG
            }



            startIndex++;
            if (startIndex > playerScripts.Count - 1) startIndex = 0;
        }
        Debug.Log("---------------------------------------------------------------");
        PhaseTester();
    }

    public void PhaseTester() //? Add 3 cards to table. Change phases later accordingly to add blinds, bets etc.
    {
        for (int i = 0; i < 5; i++)
        {    
            AddCardOnTable(i);
        }

    }

    public void AddCardOnTable(int id)
    {
        Debug.Log("ADd card on table, id: " + id);
        var cardToAdd = baseCards[0];
        cardsOnTable.Add(cardToAdd);
        tableCardScripts[id].DealCard(cardToAdd.cardNumber, cardToAdd.cardSuit);


        //DEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUG
        if (debugTableCards) Debug.Log("Card " + (cardsOnTable.Count) + ":" + cardToAdd.cardSuit.ToString() + "/" + cardToAdd.cardNumber);
        tableCards.Add(cardToAdd.cardSuit.ToString() + "/" + cardToAdd.cardNumber);
        //DEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUG


        baseCards.Remove(cardToAdd);
        if (cardsOnTable.Count == 5) //automated
        {
            Invoke(nameof(DetermineWinner),5f);
        }
    }

    public void DetermineWinner()
    {
        Debug.Log("---------------------------------------------------------------");
        PlayerScript winner = CheckWinner.DetermineWinner(cardsOnTable, playerScripts);

        if(winner == null)
        {
            Debug.Log("Tie, house wins? or split?");
        }
        else Debug.Log("Winner: " + winner.playerName.ToString() + ", with hand: " + winner.highestHand.ToString());

        ui.EnableNextRoundButton(true);
    }
    public void ResetTurn()
    {
        for(int i = 0; i < playerScripts.Count; i++)
        {
            playerScripts[i].ResetTurn();
        }

        for (int i = 0; i < tableCardScripts.Length; i++)
        {
            tableCardScripts[i].Refresh();
        }

        if (debugCardDealing)//DEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUGDEBUG
        {
            player1Cards.Clear();
            player2Cards.Clear();
        }


        //If atleast 2 has mana, continue, else gameover
        StartGame();
    }

}
