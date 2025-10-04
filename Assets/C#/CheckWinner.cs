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

        for(int i = 0;i < cardsToCheck.Count;i++)
        {
            int cardNumber = cardsToCheck[i].cardNumber;
            if (amountOfSameCards.ContainsKey(cardNumber)) amountOfSameCards[cardNumber]++;
            else amountOfSameCards.Add(cardNumber, 1);
        }

        int currentHighestSameCardNumber = 0;
        int biggestCardNumber = 0; //DOESNT WORK, RE CHECK ALL CARDS WITH THE NUMBER OF HIGHEST AMOUNT

        for(int i = 0; i < amountOfSameCards.Count;i++)
        {
            if(amountOfSameCards[i] > currentHighestSameCardNumber)
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
        NumberOfSameSuits(cardsToCheck, player);
    }
    static void NumberOfSameSuits(List<Card> cardsToCheck, PlayerScript player)
    {
        //WRITTEN ONLINE WITHOUT COMPILER -- pseudo-ish
        

        
        Dictionary<SuitEnum, int> amountOfSameSuits = new Dictionary<SuitEnum, int>();
        
        for(int i = 0; i < cardsToCheck.Count; i++)
        {
             SuitEnum cardSuit = cardsToCheck[i].cardSuit;
             if (amountOfSameSuits.ContainsKey(cardSuit)) amountOfSameSuits[cardSuit]++;
             else amountOfSameSuits.Add(cardSuit, 1);
        }

        int currentHighestSameSuitNumber = 0;
        SuitEnum highestAmountSuit;

        for(int i = 0; i < amountOfSameSuits.Count;i++)
        {
            if(amountOfSameSuits[i] > currentHighestSameCardNumber)
            {
                currentHighestSameCardNumber = amountOfSameSuits[i];
                highestAmountSuit = amountOfSameSuits[i]; //TRY GET KEY? OR IS THIS OK?
            }
        }
        if(currentHighestSamesuitnumber >= 5)
        {
            if(CheckIfBigger(player, HandEnum.Flush))
            {
                 int biggestCardNumber = 0;
                        for(int i = 0; i < cardsToCheck.Count;i++)
                {
                if(cardsToCheck[i].cardSuit == highestAmountsuit)
                {
                if(cardsToCheck[i].cardNumber > biggestCardNumber) biggestCardNumber = cardsToCheck[i].cardNumber;
            }
        
        }
        
            }
            
        }
        
       

        //Continue checking for other conditions. Or should the code check from highest hand to lowest? or just like this, check same numbers+suits first.
        //or check pairs+threes+fours in the first "check numbers" code. amount of pairs, amount of threes. cross check for fullhouse, three pairs or double threes.
        //check straight, add each card into number order and see if 5 straight, if yes, check highst card, also if straight is bigger than 5, check the highest.
        //END OF ONLINE WRITTEN CODE

    }
    static bool CheckIfBigger(PlayerScript player, HandEnum hand)
    {
        if((int)hand > (int)highestHand)
        {
            highestHand = hand;
            currentWinner = player;
            return true;
        }
        else if((int)hand == (int)highestHand)
        {
            if(player.highestCard > currentWinner.highestCard)
            {
                currentWinner = player;
                return true;
            }
            else if(player.highestCard == currentWinner.highestCard)
            {
                currentWinner = null;
            }
        }
        return false;
    }
}
