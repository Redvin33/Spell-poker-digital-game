using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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


        for (int i = 0; i < players.Count; i++)
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
        List<Card> cardsToCheck = new List<Card>(player.baseCards);
        cardsToCheck.AddRange(tableCards);

        Dictionary<int, int> amountOfSameCards = new Dictionary<int, int>();


        for (int i = 0; i < cardsToCheck.Count; i++) //Check the dictionary, if has a Key of X, add +1, else, make a dictionary key with a value of 1
        {
            int cardNumber = cardsToCheck[i].cardNumber;

            if (amountOfSameCards.ContainsKey(cardNumber)) amountOfSameCards[cardNumber]++;
            else amountOfSameCards.Add(cardNumber, 1);
        }

        int currentHighestSameCardNumber = 0;
        int biggestCardNumber = 0; //DOESNT WORK, RE CHECK ALL CARDS WITH THE NUMBER OF HIGHEST AMOUNT


        foreach(var key in amountOfSameCards.Keys)
        {
            if (amountOfSameCards[key] > currentHighestSameCardNumber)
            {
                currentHighestSameCardNumber = amountOfSameCards[key];
                biggestCardNumber = key;
            }
        }

        HandEnum highestHand = HandEnum.None;
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

        if(highestHand != HandEnum.None)
        {
            player.highestCard = biggestCardNumber;
            player.CheckHigherHand(highestHand);
        }

        //CheckIfBigger(player, highestHand); put this at the end of the checks
        NumberOfSameSuits(cardsToCheck, player);
    }
    static void NumberOfSameSuits(List<Card> cardsToCheck, PlayerScript player)
    {
        //WRITTEN ONLINE WITHOUT COMPILER -- pseudo-ish


        
        Dictionary<SuitEnum, int> amountOfSameSuits = new Dictionary<SuitEnum, int>();

        for (int i = 0; i < cardsToCheck.Count; i++)
        {
            SuitEnum cardSuit = cardsToCheck[i].cardSuit;
            if (amountOfSameSuits.ContainsKey(cardSuit)) amountOfSameSuits[cardSuit]++;
            else amountOfSameSuits.Add(cardSuit, 1);
        }

        int currentHighestSameSuitNumber = 0;
        SuitEnum highestAmountSuit = (SuitEnum)0; //Default
        int suits = System.Enum.GetValues(typeof(SuitEnum)).Length;

        for (int i = 0; i < suits; i++)
        {
            if (amountOfSameSuits.ContainsKey((SuitEnum)suits))
            {
                int suitAmountDictionary = amountOfSameSuits[(SuitEnum)suits];
                if (currentHighestSameSuitNumber < suitAmountDictionary)
                {
                    currentHighestSameSuitNumber = suitAmountDictionary;
                    highestAmountSuit = (SuitEnum)suits;
                }
                else if(currentHighestSameSuitNumber == suitAmountDictionary)
                {
                    Card highest = HighestCardAmongSuits(cardsToCheck, highestAmountSuit, (SuitEnum)suits);
                    if(highest.cardNumber > currentHighestSameSuitNumber)
                    {
                        currentHighestSameSuitNumber = highest.cardNumber;
                        highestAmountSuit = highest.cardSuit;
                    }
                }
            }
        }


        if (currentHighestSameSuitNumber >= 5) CheckIfBigger(player, HandEnum.Flush);
        CheckPairs(cardsToCheck, player);


        //Continue checking for other conditions. Or should the code check from highest hand to lowest? or just like this, check same numbers+suits first.
        //or check pairs+threes+fours in the first "check numbers" code. amount of pairs, amount of threes. cross check for fullhouse, three pairs or double threes.
        //check straight, add each card into number order and see if 5 straight, if yes, check highst card, also if straight is bigger than 5, check the highest.
        //END OF ONLINE WRITTEN CODE



    }

    static void CheckPairs(List<Card> cards, PlayerScript player)
    {
        List<Card> cardsToCheck = new List<Card>(cards);
        List<Pair> pairs = new List<Pair>();

        Dictionary<int, bool> checkPairs = new Dictionary<int, bool>();

        foreach(Card card in cardsToCheck)
        {
            if(checkPairs.ContainsKey(card.cardNumber) && checkPairs[card.cardNumber] == false)
            {
                checkPairs.Remove(card.cardNumber);
                pairs.Add(new Pair(card.cardNumber));
            }
            else checkPairs.Add(card.cardNumber, false);
        }

        if(pairs.Count > 0)
        {
            HandEnum pairHand;

            if (pairs.Count == 1) pairHand = HandEnum.OnePair;
            else if(pairs.Count == 2) pairHand = HandEnum.TwoPair;
            else pairHand = HandEnum.ThreePair;

            CheckIfBigger(player, pairHand);

            if(player.highestHand == HandEnum.ThreeOfAKind)
            {
                for(int i = 0; i < pairs.Count; i++)
                {
                    if(player.highestCard != pairs[i].highestCard)
                    {
                        CheckIfBigger(player, HandEnum.FullHouse);
                    }
                }
            }


        }

        
    }
    static bool CheckIfBigger(PlayerScript player, HandEnum hand)
    {
        if ((int)hand > (int)highestHand)
        {
            highestHand = hand;
            currentWinner = player;
            return true;
        }
        else if ((int)hand == (int)highestHand)
        {
            if (player.highestCard > currentWinner.highestCard)
            {
                currentWinner = player;
                return true;
            }
            else if (player.highestCard == currentWinner.highestCard)
            {
                currentWinner = null;
            }
        }
        return false;
    }


    static Card HighestCardAmongSuits(List<Card> cards, SuitEnum firstSuit, SuitEnum secondSuit)
    {
        int highestNumber = 0;
        Card highestCard = null;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardSuit == firstSuit || cards[i].cardSuit == secondSuit)
            {
                if (cards[i].cardNumber < highestNumber)
                {
                    highestNumber = cards[i].cardNumber;
                    highestCard = cards[i];
                }
            }
        }

        return highestCard;
    }

    static int HighestNumberAmongCards(List<Card> cards)
    {
        int highest = 0;
        return highest;
    }
}
