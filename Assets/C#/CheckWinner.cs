using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class CheckWinner
{
    static List<Card> tableCards;
    static PlayerScript currentWinner;
    static HandEnum highestHand;
    static CheckWinner()
    {
        tableCards = new List<Card>();
    }
    public static PlayerScript CheckCardLists(List<Card> newTableCards, List<PlayerScript> players)
    {
        tableCards.Clear();
        tableCards = newTableCards;

        PlayerScript currentWinner = null;


        for(int i = 0; i < players.Count; i++)
        {
            NumberOfSameCards(players[i]);
            //CheckPlayerHand(players[i]);
        }
        return currentWinner;
    }


    static void CheckPlayerHand(PlayerScript player)
    {
        
    }

    static void NumberOfSameCards(PlayerScript player)
    {
        Dictionary<int,int> amountOfSameCards = new Dictionary<int,int>();
        List<Card> cardsToCheck = new List<Card>(player.baseCards);
        cardsToCheck.AddRange(tableCards);

        for(int i = 0;i < player.baseCards.Count;i++)
        {
            int cardNumber = player.baseCards[i].cardNumber;
            if (amountOfSameCards.ContainsKey(cardNumber)) amountOfSameCards[cardNumber]++;
            else amountOfSameCards.Add(cardNumber, 1);
        }

        int currentHighestSameCardNumber = 0;
        int biggestCardNumber = 0;

        for(int i = 0; i < amountOfSameCards.Count;i++)
        {
            if(amountOfSameCards.Count > currentHighestSameCardNumber)
            {
                currentHighestSameCardNumber = amountOfSameCards[i];
                biggestCardNumber = amountOfSameCards.Single(s => s.Key == amountOfSameCards[i]).Value;
            }
            else if(amountOfSameCards.Count == currentHighestSameCardNumber)
            {
                int newCardValue = amountOfSameCards.Single(s => s.Key == amountOfSameCards[i]).Value;
                if (newCardValue > biggestCardNumber) biggestCardNumber = newCardValue;
            }
        }


        HandEnum highestHand;
        switch (currentHighestSameCardNumber)
        {
            case 5:
                highestHand = HandEnum.FiveOfAKind;
                break;
            case 4:
                highestHand = HandEnum.FourOfAKind;
                break;
            case 3:
                highestHand = HandEnum.ThreeOfAKind;
                break;
            case 2:
                highestHand = HandEnum.OnePair;
                break;
            default:
                highestHand = HandEnum.HighCard;
                break;
        }

        player.highestCard = biggestCardNumber;
        player.CheckHigherHand(highestHand);

        CheckIfBigger(player, highestHand);
        NumberOfSameSuits(cardsToCheck);
    }
    static int NumberOfSameSuits(List<Card> cardsToCheck)
    {
        int currentAmount = 0;
        Dictionary<int, int> amountOfSameCards = new Dictionary<int, int>();

        return currentAmount;
    }
    static void CheckIfBigger(PlayerScript player, HandEnum hand)
    {
        if((int)hand > (int)highestHand)
        {
            highestHand = hand;
            currentWinner = player;
        }
        else if((int)hand == (int)highestHand)
        {
            if(player.highestCard > currentWinner.highestCard)
            {
                currentWinner = player;
            }
            else if(player.highestCard == currentWinner.highestCard)
            {
                currentWinner = null;
            }
        }
    }
}
