using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public static class TieBreaker
{
    public static List<PlayerScript> DetermineTie(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        HandEnum tiedHand = tiedPlayers[0].highestHand;

        if (tiedHand == HandEnum.FiveOfAKind || tiedHand == HandEnum.FlushFive) return new List<PlayerScript>();
        else if (tiedHand == HandEnum.StraightFlush || tiedHand == HandEnum.Flush) return CompareFlush(tableCards, tiedPlayers);
        else if (tiedHand == HandEnum.FourOfAKind) return CompareSameAmount(tableCards, tiedPlayers, 4);
        else if (tiedHand == HandEnum.ThreeOfAKind) return CompareSameAmount(tableCards, tiedPlayers, 3);
        else if (tiedHand == HandEnum.OnePair) return CompareSameAmount(tableCards, tiedPlayers, 2);
        else if (tiedHand == HandEnum.HighCard) return CompareSameAmount(tableCards, tiedPlayers, 1);
        else if (tiedHand == HandEnum.FlushHouse || tiedHand == HandEnum.FullHouse) return CompareFullHouse(tableCards, tiedPlayers);
        else if (tiedHand == HandEnum.Straight || tiedHand == HandEnum.StraightFlush) return CompareStraight(tableCards, tiedPlayers);
        else if (tiedHand == HandEnum.TwoPair) return CompareTwoPair(tableCards, tiedPlayers);

        Debug.Log("returining empty");
        return new List<PlayerScript>();
    }
    static List<PlayerScript> CompareFlush(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        SuitEnum flushSuit = MostSuits(tableCards);
        List<PlayerScript> winners = new List<PlayerScript>();

        for(int i = 0; i < tableCards.Count; i++)
        {
            Debug.Log("Table card " + (i + 1) + " is: " + tableCards[i].cardNumber);
        }
        for(int i = 0; i < tiedPlayers.Count; i++)
        {
            for (int j = 0; j < tiedPlayers[i].baseCards.Count; j++)
            {
                Debug.Log("Player " + tiedPlayers[i].playerName.ToString() + " card " + (i + 1) + " is: " + tiedPlayers[i].baseCards[j].cardNumber);
            }

        }

        int biggestCard = 0;

        for (int i = 0; i < tiedPlayers.Count; i++)
        {
            int biggest = BiggestAmongSuits(tableCards, tiedPlayers[i], flushSuit);
            Debug.Log("Biggest among suit:" + flushSuit.ToString() + ", on player: " + tiedPlayers[i].playerID.ToString() + ". Players biggest: " + biggest + ". And current Biggest: " + biggestCard + ". winners count: " + winners.Count);
            if (biggest > biggestCard)
            {
                if(winners.Count > 0) winners.Clear();
                biggestCard = biggest;
                winners.Add(tiedPlayers[i]);
            }
            else if (biggest == biggestCard) winners.Add(tiedPlayers[i]);
        }

        return winners;
    }
    static List<PlayerScript> CompareFullHouse(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        List<PlayerScript> winners = new List<PlayerScript>();
        Dictionary<PlayerScript, int> highestThree = new Dictionary<PlayerScript, int>();
        int biggest = 0;

        int amountToCheck = 3;

        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < tiedPlayers.Count; i++)
            {
                List<Card> playerCards = new List<Card>(tableCards);
                playerCards.AddRange(tiedPlayers[i].baseCards);
                highestThree.Add(tiedPlayers[i], GetBiggestSameOfKinds(playerCards, amountToCheck));
            }

            foreach (var keys in highestThree.Keys)
            {
                if (highestThree[keys] > biggest)
                {
                    winners.Clear();
                    biggest = highestThree[keys];
                    winners.Add(keys);
                }
                else if (highestThree[keys] == biggest) winners.Add(keys);
            }

            if (winners.Count > 1)
            {
                biggest = 0;
                amountToCheck--;
                highestThree.Clear();
                winners.Clear();
            }
            else return winners;
        }
        return winners;
    }
    static List<PlayerScript> CompareStraight(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        List<PlayerScript> winners = new List<PlayerScript>();
        Dictionary<PlayerScript, List<int>> playerCards = new Dictionary<PlayerScript, List<int>>();
        int biggest = 0;

        for(int i = 0; i < tiedPlayers.Count; i++)
        {
            List<Card> cardsToCheck = new List<Card>(tableCards);
            cardsToCheck.AddRange(tiedPlayers[i].baseCards);


            for (int j = 0; j < cardsToCheck.Count; j++)
            {
                playerCards[tiedPlayers[i]].Add(cardsToCheck[j].cardNumber);
                if (cardsToCheck[j].cardNumber == 14) playerCards[tiedPlayers[i]].Add(1); //If the card is an Ace (=14), add 1. Ace = 1 & 14
            }

            playerCards[tiedPlayers[i]].Sort();
            playerCards[tiedPlayers[i]].Reverse();

            int counter = 0;
            int startingCard = 0;

            for (int j = 0; j < playerCards[tiedPlayers[i]].Count; j++)
            {
                if (playerCards[tiedPlayers[i]][j] != playerCards[tiedPlayers[i]].Last())
                {
                    if (playerCards[tiedPlayers[i]][j] == playerCards[tiedPlayers[i]][j + 1] + 1)
                    {
                        if (counter == 0)
                        {
                            counter += 2;
                            startingCard = playerCards[tiedPlayers[i]][j];
                        }
                        else counter++;
                    }
                    else counter = 0;
                }
            }

            if(counter >= 5)
            {
                if(playerCards[tiedPlayers[i]][0] > biggest)
                {
                    if (winners.Count > 0) winners.Clear();
                    biggest = startingCard;
                    winners.Add(tiedPlayers[i]);
                }
                else if(playerCards[tiedPlayers[i]][0] == biggest) winners.Add(tiedPlayers[i]);
            }
        }

        return winners;
    }
    static int GetBiggestSameOfKinds(List<Card> checkFrom, int sameKindAmount)
    {
        Dictionary<int, int> amounts = new Dictionary<int, int>();

        for (int i = 0; i < checkFrom.Count; i++)
        {
            if (amounts.ContainsKey(checkFrom[i].cardNumber)) amounts[checkFrom[i].cardNumber]++;
            else amounts.Add(checkFrom[i].cardNumber, 1);
        }

        int biggest = 0;
        foreach (var keys in amounts.Keys)
        {
            if (amounts[keys] == sameKindAmount && keys > biggest) biggest = keys;
        }

        return biggest;
    } 
    static List<PlayerScript> CompareSameAmount(List<Card> tableCards, List<PlayerScript> tiedPlayers, int sameKindAmount)
    {
        List<PlayerScript> winners = new List<PlayerScript>();

        Dictionary<PlayerScript, int> highestOfSame = new Dictionary<PlayerScript, int>();
        Dictionary<PlayerScript, List<Card>> playerCards = new Dictionary<PlayerScript, List<Card>>();

        int biggestNumber = 0;


        for (int i = 0; i < tiedPlayers.Count; i++)
        {
            playerCards.Add(tiedPlayers[i], new List<Card>(tableCards));
            playerCards[tiedPlayers[i]].AddRange(tiedPlayers[i].baseCards);
            highestOfSame.Add(tiedPlayers[i], GetBiggestSameOfKinds(playerCards[tiedPlayers[i]], sameKindAmount));
        }

        foreach (var keys in highestOfSame.Keys)
        {
            if (highestOfSame[keys] > biggestNumber)
            {
                winners.Clear();
                biggestNumber = highestOfSame[keys];
                winners.Add(keys);
            }
            else if (highestOfSame[keys] == biggestNumber) winners.Add(keys);
        }
        Debug.Log("First check on samekindamount, winenr count: " + winners.Count);

        if (winners.Count == 1) return winners;
        else
        {

            Dictionary<PlayerScript, List<int>> otherCards = new Dictionary<PlayerScript, List<int>>();
            int leftOver = 5 - sameKindAmount;

            for(int i = 0;i < winners.Count; i++)
            {
                foreach(var keys in playerCards.Keys)
                {
                    if (winners[i] == keys)
                    {
                        List<int> cardsToCheck = new List<int>();

                        for (int j = 0; j < playerCards[keys].Count; j++)
                        {
                            if (playerCards[keys][j].cardNumber != biggestNumber)
                            {
                                cardsToCheck.Add(playerCards[keys][j].cardNumber);
                            }
                        }
                        cardsToCheck.Sort();
                        cardsToCheck.Reverse();

                        otherCards.Add(keys, new List<int>(cardsToCheck));
                        break;
                    }
                }
            }


            winners.Clear();
            List<PlayerScript> previousWinners = new List<PlayerScript>();
            for( int i = 0; i < leftOver; i++)
            {
                biggestNumber = 0;
                foreach (var keys in otherCards.Keys)
                {
                    if (otherCards[keys][i] > biggestNumber)
                    {
                        if(winners.Count > 0) winners.Clear();
                        biggestNumber = otherCards[keys][i];
                        winners.Add(keys);
                    }
                    else if (otherCards[keys][i] == biggestNumber) winners.Add(keys);
                }
                Debug.Log("Leftover check, leftover: " + leftOver + ". At the end of iteration, winenr amount: " + winners.Count);
                if (winners.Count == 1) return winners;
                else if(winners.Count > 0)
                {
                    if (previousWinners.Count > 0) previousWinners.Clear();
                    previousWinners.AddRange(winners);
                }

            }

            Debug.Log("Returning count on cehck same amount: " + winners.Count);
            if (previousWinners.Count > 0) return previousWinners;
            else return winners;
        }
    }
    static List<PlayerScript> CompareTwoPair(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        List<PlayerScript> winners = new List<PlayerScript>();
        List<TwoPairHelper> helpers = new List<TwoPairHelper>();

        for(int i = 0; i < tiedPlayers.Count; i++)
        {
            List<Card> cardsToCheck = new List<Card>(tableCards);
            cardsToCheck.AddRange(tiedPlayers[i].baseCards);

            TwoPairHelper current = new TwoPairHelper(tiedPlayers[i]);
            helpers.Add(current);

            for (int j = 0; j < cardsToCheck.Count; j++)
            {
                if (current.cardPerNumber.ContainsKey(cardsToCheck[j].cardNumber)) current.cardPerNumber[cardsToCheck[j].cardNumber]++;
                else current.cardPerNumber.Add(cardsToCheck[j].cardNumber, 1);
            }

            current.CheckPairs();
        }

        int biggest;
        List<PlayerScript> previousWinners = new List<PlayerScript>();

        for (int i = 0; i < 2; i++)
        {
            biggest = 0;

            for (int j = 0; j < helpers.Count; j++)
            {
                if (helpers[j].GetPairIteration(i) > biggest)
                {
                    if (winners.Count > 0) winners.Clear();
                    biggest = helpers[j].GetPairIteration(i);
                    winners.Add(helpers[j].player);
                }
                else if(helpers[j].GetPairIteration(i) == biggest) winners.Add(helpers[j].player);
            }

            if (winners.Count == 1) return winners;
            else if(winners.Count > 0)
            {            
                if (previousWinners.Count > 0) previousWinners.Clear();
                previousWinners.AddRange(winners);            
            }
        }

        if(winners.Count > 1)
        {
            biggest = 0;

            for (int j = 0; j < helpers.Count; j++)
            {
                if (helpers[j].leftOver > biggest)
                {
                    if (winners.Count > 0) winners.Clear();
                    biggest = helpers[j].leftOver;
                    winners.Add(helpers[j].player);
                }
                else if (helpers[j].leftOver == biggest) winners.Add(helpers[j].player);
            }
        }
        return winners;
    }
    static List<int> GetFiveHighestDescendingOrder(List<Card> cards)
    {
        List<int> toReturn = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            toReturn.Add(cards[i].cardNumber);
        }

        toReturn.Sort();
        toReturn.Reverse();

        return toReturn;
    }
    static SuitEnum MostSuits(List<Card> cards)
    {
        Dictionary<SuitEnum, int> suits = new Dictionary<SuitEnum, int>();
        SuitEnum mostSuits = (SuitEnum)0;

        for (int i = 0; i < cards.Count; i++)
        {
            SuitEnum cardSuit = cards[i].cardSuit;

            if (suits.ContainsKey(cardSuit)) suits[cardSuit]++;
            else suits.Add(cardSuit, 1);
        }

        int largestSuitAmount = 0;


        foreach (var key in suits.Keys)
        {
            if (suits[key] > largestSuitAmount)
            {
                largestSuitAmount = suits[key];
                mostSuits = key;
            }          
        }

        Debug.Log("MOSTSUITS TIE BREAKER: " + mostSuits.ToString() + ", amount: " + largestSuitAmount);
        return mostSuits;
    }
    static int BiggestAmongSuits(List<Card> cards, PlayerScript player, SuitEnum suit)
    {
        List<Card> cardsToCheck = new List<Card>();
        cardsToCheck.AddRange(cards);
        cardsToCheck.AddRange(player.baseCards);

        int biggest = 0;

        for(int i = 0; i < cardsToCheck.Count; i++)
        {
            if (cardsToCheck[i].cardSuit == suit && cardsToCheck[i].cardNumber > biggest) biggest = cardsToCheck[i].cardNumber;
        }

        return biggest;
    }
    static int BiggestAmongSameNumbers(List<Card> cards, PlayerScript player) //check the highest amount of each number, and returns the biggest among the highest
    {
        cards.AddRange(player.baseCards);

        Dictionary<int, int> values = new Dictionary<int, int>();

        for (int i = 0; i < cards.Count; i++)
        {
            int cardNumber = cards[i].cardNumber;

            if (values.ContainsKey(cardNumber))
            {
                values[cardNumber]++;
            }
            else values.Add(cardNumber, 1);
        }

        int biggestAmount = 0;
        int biggestNumber = 0;

        foreach (var keys in values.Keys)
        {
            if (values[keys] > biggestAmount)
            {
                biggestAmount = values[keys];
                biggestNumber = keys;
            }
            else if(values[keys] == biggestAmount && keys > biggestNumber) biggestNumber = keys;
        }

        return biggestNumber;
    }
    static int GetBiggestCard(List<Card> cards)
    {
        int biggest = 0;

        for(int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardNumber > biggest) biggest = cards[i].cardNumber;
        }
        return biggest;
    }
}