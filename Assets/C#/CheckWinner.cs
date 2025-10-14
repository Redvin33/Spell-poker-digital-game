using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Diagnostics;




public static class CheckWinner
{
    static List<Card> tableCards;

    static CheckWinner()
    {
        tableCards = new List<Card>();
    }
    public static PlayerScript DetermineWinner(List<Card> newTableCards, List<PlayerScript> players)
    {
        tableCards = newTableCards;

        for (int i = 0; i < players.Count; i++)
        {
            CheckPlayerHand(players[i]);
        }

        PlayerScript currentWinner = null;
        List<PlayerScript> possibleTies = new List<PlayerScript>();

        for(int i = 0; i < players.Count; i++)
        {
            if (currentWinner != null)
            {
                if ((int)players[i].highestHand > (int)currentWinner.highestHand)
                {
                    currentWinner = players[i];
                    possibleTies.Clear();
                }
                else if((int)players[i].highestHand == (int)currentWinner.highestHand)
                {
                    if (!possibleTies.Contains(players[i])) possibleTies.Add(players[i]);
                    if (!possibleTies.Contains(currentWinner)) possibleTies.Add(currentWinner);
                    currentWinner = null;
                }
            }
            else currentWinner = players[i];
        }
  

        if(possibleTies.Count > 0) return TieBreaker.DetermineTie(tableCards, possibleTies);
        else return currentWinner;
    }


    static void CheckPlayerHand(PlayerScript player)
    {
        Debug.Log("---------------------------------------------------------------");
        Debug.Log("Checking player " + player.playerID);    
        List<Card> cardsToCheck = new List<Card>(player.baseCards);
        cardsToCheck.AddRange(tableCards);

        HandEnum currentHighestHand = CheckSameValues(cardsToCheck);

        Debug.Log("Returned ''Check Same Values'' as: " + currentHighestHand.ToString());
        if((int)currentHighestHand  < (int)HandEnum.FiveOfAKind)
        {
            Debug.Log("No five of a kind, checking flushes. Current highest: " + currentHighestHand.ToString());

            HandEnum flushType = CheckFlush(cardsToCheck);
            if ((int)flushType > (int)currentHighestHand) currentHighestHand = flushType;

            HandEnum fullHouse = CheckFullHouse(cardsToCheck);
            if ((int)fullHouse > (int)currentHighestHand) currentHighestHand = fullHouse;
            Debug.Log("Checked fullhouse, returned: " + fullHouse.ToString());
            Debug.Log("Checked flushes and fullhouse, current highest: " + currentHighestHand.ToString());
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
        int highestAmount = 0;
        int biggestCardNumber = 0;

        for (int i = 0; i < cardsToCheck.Count; i++) //Check the dictionary, if has a Key of X, add +1, else, make a dictionary key with a value of 1
        {
            int cardNumber = cardsToCheck[i].cardNumber;

            if (amountOfSameCards.ContainsKey(cardNumber))
            {
                amountOfSameCards[cardNumber]++;
                if(amountOfSameCards[cardNumber] > highestAmount)
                {
                    highestAmount = amountOfSameCards[cardNumber];
                    biggestCardNumber = cardNumber;
                }
            }
            else amountOfSameCards.Add(cardNumber, 1);
        }

        int currentHighestSameCardNumber = 0;


        foreach (var key in amountOfSameCards.Keys)
        {
            if (amountOfSameCards[key] > currentHighestSameCardNumber)
            {
                currentHighestSameCardNumber = amountOfSameCards[key];
            }
        }

        switch (currentHighestSameCardNumber)
        {
            case >4:
                {
                    List<Card> checkFiveForFlush = new List<Card>();
                    for(int j = 0; j < cardsToCheck.Count; j++)
                    {
                        if (cardsToCheck[j].cardNumber == biggestCardNumber) checkFiveForFlush.Add(cardsToCheck[j]);
                    }
                    if (IsFlush(checkFiveForFlush)) return HandEnum.FlushFive;
                    else return HandEnum.FiveOfAKind;
                }
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
    
    static bool IsFlush(List<Card> cards)
    {
        Dictionary<SuitEnum, int> suitsAmount = new Dictionary<SuitEnum, int>();

        for (int i = 0; i < cards.Count; i++)
        {
            if (suitsAmount.ContainsKey(cards[i].cardSuit)) suitsAmount[cards[i].cardSuit]++;
            else suitsAmount.Add(cards[i].cardSuit, 1);
        }

        foreach(var key in suitsAmount.Keys)
        {
            if (suitsAmount[key] >= 5)
            {
                 return true;
            }
        }
        return false;
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
            }
            else amountOfSameSuits.Add(cardSuit, 1);
        }

        foreach(var key in amountOfSameSuits.Keys)
        {
            if (amountOfSameSuits[key] >= 5)
            {
                //flush achieved, check straigh flush
                List<Card> flushCards = new List<Card>();

                for (int i = 0; i < cardsToCheck.Count; i++) // add all flush suits to a list
                {
                    if (cardsToCheck[i].cardSuit == key) flushCards.Add(cardsToCheck[i]);
                }

                if (CheckStraight(flushCards)) return HandEnum.StraightFlush;
                else return HandEnum.Flush;
            }
        }
        return HandEnum.None;
    }

    static bool CheckStraight(List<Card> cards)
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            numbers.Add(cards[i].cardNumber);
            if (cards[i].cardNumber == 14) numbers.Add(1); //If the card is an Ace (=14), add 1. Ace = 1 & 14
        }

        numbers.Sort();
        numbers.Reverse();
        int counter = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            if (numbers[i] != numbers.Last())
            {
                if (numbers[i] == numbers[i + 1] + 1)
                {
                    if (counter == 0) counter += 2;
                    else counter++;
                }
                else counter = 0;
            }
        }

        if (counter >= 5) return true;
        else return false;
    }

    static HandEnum CheckFullHouse(List<Card> cards)
    {
        //card numbers with +3, if 2, go on. Else, if 1x 3-4, check for 2´s.
        Dictionary<int, List<Card>> sameCards = new Dictionary<int, List<Card>>();

        for (int i = 0; i < cards.Count; i++)
        {
            if (sameCards.ContainsKey(cards[i].cardNumber))
            {
                sameCards[cards[i].cardNumber].Add(cards[i]);
            }
            else
            {
                sameCards.Add(cards[i].cardNumber, new List<Card>());
                sameCards[cards[i].cardNumber].Add(cards[i]);
            }
        }

        //Check how many +threes there are in the dictionary
        int threes = 0;

        foreach(var value in sameCards.Values)
        {
            if (value.Count >= 3) threes++;
        }

        if (threes > 1)
        {
            //we have a fullhouse (atleast one 3, and atleast one 3-4)
            List<Card> fullHouseCards = new List<Card>();

            foreach (var keys in sameCards.Keys)
            {
                if (sameCards[keys].Count >= 3) fullHouseCards.AddRange(sameCards[keys]);
            }

            //checking flushhouse
            if (IsFlush(fullHouseCards)) return HandEnum.FlushHouse;
            else return HandEnum.FullHouse;
        }
        else if (threes == 1)
        {
            //we have one of atleast 3-5.
            List<Card> fullHouseCards = new List<Card>();

            foreach (var keys in sameCards.Keys)
            {
                if (sameCards[keys].Count >= 3)
                {
                    fullHouseCards.AddRange(sameCards[keys]);

                    for (int j = 0; j < sameCards[keys].Count; j++)
                    {
                        cards.Remove(sameCards[keys][j]);
                    }
                    //removing the 3-5 cards from cardsList, checking them again for twos on the next if-loop
                    break;
                }
            }

            sameCards.Clear();

            for (int i = 0; i < cards.Count; i++)
            {
                if (sameCards.ContainsKey(cards[i].cardNumber))
                {
                    sameCards[cards[i].cardNumber].Add(cards[i]);
                }
                else
                {
                    sameCards.Add(cards[i].cardNumber, new List<Card>());
                    sameCards[cards[i].cardNumber].Add(cards[i]);
                }
            }

            //checking if there are any twos in the remaining list
            int twos = 0;

            foreach (var value in sameCards.Values)
            {
                if (value.Count >= 2) twos++;
            }

            if (twos > 0)
            {
                //we have a fullhouse. One 3+. and atleast one 2+. (3+2+2 possible)
                foreach (var keys in sameCards.Keys)
                {
                    if (sameCards[keys].Count >= 2) fullHouseCards.AddRange(sameCards[keys]);
                }

                //checking flushhouse
                if (IsFlush(fullHouseCards)) return HandEnum.FlushHouse;
                else return HandEnum.FullHouse;

            }
        }

        return HandEnum.None;
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
