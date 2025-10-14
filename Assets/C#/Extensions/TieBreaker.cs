using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TieBreaker
{
    public static PlayerScript DetermineTie(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        HandEnum tiedHand = tiedPlayers[0].highestHand;
        if (tiedHand == HandEnum.FiveOfAKind || tiedHand == HandEnum.FlushFive) return null;
        else if (tiedHand == HandEnum.StraightFlush) return CompareStraightFlush(tableCards, tiedPlayers);
        else if (tiedHand == HandEnum.FourOfAKind) return CompareSameAmount(tableCards, tiedPlayers, 4);
        else if (tiedHand == HandEnum.ThreeOfAKind) return CompareSameAmount(tableCards, tiedPlayers, 3);
        else if (tiedHand == HandEnum.OnePair) return CompareSameAmount(tableCards, tiedPlayers, 2);
        else if (tiedHand == HandEnum.HighCard) return CompareSameAmount(tableCards, tiedPlayers, 1);
        else if (tiedHand == HandEnum.FlushHouse || tiedHand == HandEnum.FullHouse) return CompareFullHouse(tableCards, tiedPlayers);
        //and two pair, straight, flush
        return null;
    }

    static PlayerScript CompareStraightFlush(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        SuitEnum flushSuit = MostSuits(tableCards);

        int biggestCard = 0;
        PlayerScript leadingPlayer = null;

        for (int i = 0; i < tiedPlayers.Count; i++)
        {
            int biggest = BiggestAmongSuits(tableCards, tiedPlayers[i], flushSuit);
            if (biggest > biggestCard)
            {
                biggestCard = biggest;
                leadingPlayer = tiedPlayers[i];
            }
            else if (biggest == biggestCard) leadingPlayer = null;
        }

        return leadingPlayer;
    }

    static PlayerScript CompareFullHouse(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        Dictionary<PlayerScript, int> highestThree = new Dictionary<PlayerScript, int>();
        int biggest = 0;
        PlayerScript winner = null;

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
                    biggest = highestThree[keys];
                    winner = keys;
                }
                else if (highestThree[keys] == biggest) winner = null;
            }

            if (winner == null)
            {
                biggest = 0;
                amountToCheck--;
                highestThree.Clear();
            }
            else return winner;
        }
        return null;
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
    static PlayerScript CompareSameAmount(List<Card> tableCards, List<PlayerScript> tiedPlayers, int sameKindAmount)
    {

        //LEFT HERE!
        Dictionary<PlayerScript, int> highestOfSame = new Dictionary<PlayerScript, int>();

        int biggestNumber = 0;
        PlayerScript leadingPlayer = null;


        for (int i = 0; i < tiedPlayers.Count; i++)
        {
            List<Card> playerCards = new List<Card>(tableCards);
            playerCards.AddRange(tiedPlayers[i].baseCards);
            highestOfSame.Add(tiedPlayers[i], GetBiggestSameOfKinds(playerCards, sameKindAmount));
        }

        foreach (var keys in highestOfSame.Keys)
        {
            if (highestOfSame[keys] > biggestNumber)
            {
                biggestNumber = highestOfSame[keys];
                leadingPlayer = keys;
            }
            else if (highestOfSame[keys] == biggestNumber) leadingPlayer = null;
        }

        if (leadingPlayer != null) return leadingPlayer;
        else
        {
            return null;
        }

        // int leftOverFour = IsCommunityFour(tableCards);

        // if (leftOverFour != 0)
        // {
        //     //fours on community
        //     biggestNumber = leftOverFour;

        //     for (int i = 0; i < tiedPlayers.Count; i++)
        //     {
        //         for (int j = 0; j < tiedPlayers[i].baseCards.Count; j++)
        //         {
        //             if (tiedPlayers[i].baseCards[j].cardNumber > biggestNumber)
        //             {
        //                 biggestNumber = tiedPlayers[i].baseCards[j].cardNumber;
        //                 leadingPlayer = tiedPlayers[i];
        //             }
        //             else if (tiedPlayers[i].baseCards[j].cardNumber == biggestNumber && leadingPlayer != tiedPlayers[i]) leadingPlayer = null;
        //         }
        //     }
        // }
        // else
        // {
        //     //Create func, which iteraters through all cards from highest to lowest, continue for ties.
        //     Dictionary<PlayerScript, List<Card>> cardsToCheck = new Dictionary<PlayerScript, List<Card>>();

        //     for(int i = 0; i < tiedPlayers.Count; i++)
        //     {
        //         List<Card> highestPlayerCards = new List<Card>();
        //         cardsToCheck.Add(tiedPlayers[i], highestPlayerCards);
        //     }


        //     for (int i = 0; i < tiedPlayers.Count; i++)
        //     {



        //         int biggest = BiggestAmongSameNumbers(tableCards, tiedPlayers[i]);
        //         if (biggest > biggestNumber)
        //         {
        //             biggestNumber = biggest;
        //             leadingPlayer = tiedPlayers[i];
        //         }
        //         else if (biggest == biggestNumber) leadingPlayer = null;
        //     }
        // }



        // return leadingPlayer;
    }

    static PlayerScript ComparePair(List<Card> tableCards, PlayerScript currentWinner, PlayerScript player2)
    {

        //???


        return currentWinner;
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
    //static bool CheckIfBigger(PlayerScript player, HandEnum hand)
    //{
    //    if ((int)hand > (int)highestHand)
    //    {
    //        highestHand = hand;
    //        currentWinner = player;
    //        return true;
    //    }
    //    else if ((int)hand == (int)highestHand)
    //    {
    //        if (player.highestCard > currentWinner.highestCard)
    //        {
    //            currentWinner = player;
    //            return true;
    //        }
    //        else if (player.highestCard == currentWinner.highestCard)
    //        {
    //            currentWinner = null;
    //        }
    //    }
    //    return false;
    //}


    //static Card HighestCardAmongSuits(List<Card> cards, SuitEnum firstSuit, SuitEnum secondSuit)
    //{
    //    int highestNumber = 0;
    //    Card highestCard = null;

    //    for (int i = 0; i < cards.Count; i++)
    //    {
    //        if (cards[i].cardSuit == firstSuit || cards[i].cardSuit == secondSuit)
    //        {
    //            if (cards[i].cardNumber < highestNumber)
    //            {
    //                highestNumber = cards[i].cardNumber;
    //                highestCard = cards[i];
    //            }
    //        }
    //    }

    //    return highestCard;
    //}


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
        cards.AddRange(player.baseCards);
        int biggest = 0;

        for(int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardSuit == suit && cards[i].cardNumber > biggest) biggest = cards[i].cardNumber;
        }

        return biggest;
    }
    static int IsCommunityFour(List<Card> cards)
    {
        Dictionary<int, int> values = new Dictionary<int, int>();
        bool hasFour = false;
        for(int i = 0;i < cards.Count;i++)
        {
            int cardNumber = cards[i].cardNumber;

            if (values.ContainsKey(cardNumber))
            {
                values[cardNumber]++;
                if (values[cardNumber] == 4) hasFour = true;

            }
            else values.Add(cardNumber, 1);
        }

        if(hasFour)
        {
            foreach(var keys in values.Keys)
            {
                if (values[keys] == 1) return values[keys];
            }
        }
        return 0;
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
}
