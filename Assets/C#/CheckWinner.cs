using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Experimental.GraphView;
public static class CheckWinner
{
    static List<Card> tableCards;
    static PlayerScript currentWinner;
    static HandEnum highestHand;
    static CheckWinner()
    {
        tableCards = new List<Card>();
    }
    public static PlayerScript DetermineWinner(List<Card> newTableCards, List<PlayerScript> players)
    {
        tableCards = newTableCards;

        Debug.Log("Determining winner, tablecards: " + tableCards.Count);
        for (int i = 0; i < players.Count; i++)
        {
            CheckPlayerHand(players[i]);
        }

        PlayerScript currentWinner = null;

        for(int i = 0; i < players.Count; i++)
        {
            if (currentWinner != null)
            {
                if ((int)players[i].highestHand > (int)currentWinner.highestHand)
                {
                    currentWinner = players[i];
                }
                else if((int)players[i].highestHand == (int)currentWinner.highestHand)
                {
                    //check highest card among hands... phew... throw list of cards and handenum to a function which return highest card for both, compare, declare winner.
                    Debug.Log("There was a tie: " + currentWinner.highestHand.ToString());
                }
            }
            else currentWinner = players[i];
        }
        tableCards.Clear();
        return currentWinner;
    }


    static void CheckPlayerHand(PlayerScript player)
    {
        Debug.Log("---------------------------------------------------------------");
        Debug.Log("Checking player " + player.playerID);    
        List<Card> cardsToCheck = new List<Card>(player.baseCards);
        cardsToCheck.AddRange(tableCards);

        HandEnum currentHighestHand = CheckSameValues(cardsToCheck);

        Debug.Log("Returned ''Check Same Values'' as: " + currentHighestHand.ToString());
        if(currentHighestHand != HandEnum.FiveOfAKind)
        {
            Debug.Log("No five of a kind, checking flushes. Current highest: " + currentHighestHand.ToString());

            HandEnum flushType = CheckFlush(cardsToCheck);

            if ((int)flushType > (int)currentHighestHand)
            {
                if(flushType == HandEnum.Flush)
                {
                    if(currentHighestHand != HandEnum.FourOfAKind)
                    {
                        if (CheckFullHouse(cardsToCheck)) currentHighestHand = HandEnum.FullHouse;
                        else currentHighestHand = flushType;
                    }
                }
                else currentHighestHand = flushType;
            }

            Debug.Log("Checked flushes and fullhouse, currenh highest: " + currentHighestHand.ToString());
            if((int)currentHighestHand < (int)HandEnum.TwoPair)
            {
                Debug.Log("Checking two pairs");
                if(CheckTwoPairs(cardsToCheck))
                {
                    currentHighestHand = HandEnum.TwoPair;
                }
            }
        }


        Debug.Log("Checking, player: " + player.playerName.ToString() + ", highest hand: " + currentHighestHand.ToString());
        player.SetHighestHand(currentHighestHand);
    }

    static HandEnum CheckSameValues(List<Card> cardsToCheck)
    {

        Dictionary<int, int> amountOfSameCards = new Dictionary<int, int>();

        Debug.Log("Cards to check amount: " + cardsToCheck.Count);
        for(int i = 0; i < cardsToCheck.Count; i++)
        {
            Debug.Log("Card: " + cardsToCheck[i].cardNumber.ToString() + "/" + cardsToCheck[i].cardSuit.ToString());
        }





        for (int i = 0; i < cardsToCheck.Count; i++) //Check the dictionary, if has a Key of X, add +1, else, make a dictionary key with a value of 1
        {
            int cardNumber = cardsToCheck[i].cardNumber;

            if (amountOfSameCards.ContainsKey(cardNumber)) amountOfSameCards[cardNumber]++;
            else amountOfSameCards.Add(cardNumber, 1);
        }

        int currentHighestSameCardNumber = 0;


        foreach(var key in amountOfSameCards.Keys)
        {
            if (amountOfSameCards[key] > currentHighestSameCardNumber)
            {
                currentHighestSameCardNumber = amountOfSameCards[key];
            }
        }

        Debug.Log("currentHighestSameCardNumber: " + currentHighestSameCardNumber.ToString());
        switch (currentHighestSameCardNumber)
        {
            case 5:
                return HandEnum.FiveOfAKind;
            case 4:
                return HandEnum.FourOfAKind;
            case 3:
                return HandEnum.ThreeOfAKind;
            case 2:
                return HandEnum.OnePair;
            default:
                return HandEnum.HighCard;
        }
    }
    static HandEnum CheckFlush(List<Card> cardsToCheck)
    {     
        Dictionary<SuitEnum, int> amountOfSameSuits = new Dictionary<SuitEnum, int>();

        for (int i = 0; i < cardsToCheck.Count; i++)
        {
            SuitEnum cardSuit = cardsToCheck[i].cardSuit;

            if (amountOfSameSuits.ContainsKey(cardSuit))
            {
                amountOfSameSuits[cardSuit]++;
                if (amountOfSameSuits[cardSuit] >= 5)
                {
                    //Flush achieved, check for royal & straight
                    List<Card> flushCards = new List<Card>();

                    for(int j = 0; j < cardsToCheck.Count; j++) // add all flush suits to a list
                    {
                        if (cardsToCheck[j].cardSuit == cardSuit) flushCards.Add(cardsToCheck[j]);
                    }

                    if (CheckStraight(flushCards))// check the list if it contains straight, if yes, check if the flush contains Ace. If no straight, return flush.
                    {
                        if (HighestNumberAmongCards(flushCards) == 14) return HandEnum.RoyalFlush;
                        else return HandEnum.StraightFlush;
                    }
                    else return HandEnum.Flush;
                }
            }
            else amountOfSameSuits.Add(cardSuit, 1);
        }

        return HandEnum.None;
    }

    static bool CheckStraight(List<Card> cards)
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            numbers.Add(cards[i].cardNumber);
        }

        numbers.Sort();

        for (int i = 0; i < numbers.Count; i++)
        {
            if (numbers[i] != numbers.Last())
            {
                if (numbers[i] != numbers[i + 1])
                {
                    if (numbers.Count - i >= 4) continue;
                    else return false;
                }
            }
        }
        return true;
    }

    static bool CheckFullHouse(List<Card> cards)
    {
        Dictionary<int, int> cardDic = new Dictionary<int, int>();


        for(int i = 0; i < cards.Count; i++)
        {
            if (cardDic.ContainsKey(cards[i].cardNumber))
            {
                cardDic[cards[i].cardNumber]++;
                if (cardDic[cards[i].cardNumber] >= 3)
                {
                    List<Card> restOfCards = new List<Card>();

                    for(int j = 0; j < cards.Count; j++)
                    {
                        if (cards[j].cardNumber != cards[i].cardNumber)
                        {
                            restOfCards.Add(cards[j]);
                        }
                    }

                    if (restOfCards.Count >= 2)
                    {
                        cardDic.Clear();

                        for (int k = 0; k < restOfCards.Count; k++)
                        {
                            if (cardDic.ContainsKey(cards[k].cardNumber)) 
                            {
                                cards[k].cardNumber++;
                                if (cards[k].cardNumber >= 2)
                                {
                                    return true;
                                }
                            }
                            else cardDic.Add(cards[k].cardNumber, 1);
                        }
                    }
                    else break;

                }
            }
            else cardDic.Add(cards[i].cardNumber, 1);
        }


        return false;
    }
    static bool CheckTwoPairs(List<Card> cards)
    {
        Dictionary<int, int> checkPairs = new Dictionary<int, int>();
        int pairs = 0;

        for(int i = 0; i < cards.Count; i++)
        {
            if (checkPairs.ContainsKey(cards[i].cardNumber))
            {
                checkPairs[cards[i].cardNumber]++;
                if (checkPairs[cards[i].cardNumber] >= 2)
                {
                    pairs++;
                }
            }
            else checkPairs.Add(cards[i].cardNumber, 1);
        }

        return pairs >= 2;
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

        for(int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardNumber > highest) highest = cards[i].cardNumber;
        }

        return highest;
    }
}
