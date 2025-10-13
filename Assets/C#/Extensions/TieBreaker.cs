using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TieBreaker
{
    public static PlayerScript DetermineTie(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {
        Debug.Log("trying to determine tie, returning player1 for now");
        HandEnum tiedHand = tiedPlayers[0].highestHand;


        if (tiedHand == HandEnum.FiveOfAKind) return null;
        else if (tiedHand == HandEnum.StraightFlush) return CompareStraightFlush(tableCards, tiedPlayers);
        else if (tiedHand == HandEnum.FourOfAKind)
        {

        }


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
    static PlayerScript CompareFour(List<Card> tableCards, List<PlayerScript> tiedPlayers)
    {

        int biggestNumber = 0;
        PlayerScript leadingPlayer = null;

        int leftOverFour = IsCommunityFour(tableCards);

        if (leftOverFour != 0)
        {
            //fours on community
            biggestNumber = leftOverFour;

            for (int i = 0; i < tiedPlayers.Count; i++)
            {
                for (int j = 0; j < tiedPlayers[i].baseCards.Count; j++)
                {
                    if (tiedPlayers[i].baseCards[j].cardNumber > biggestNumber)
                    {
                        biggestNumber = tiedPlayers[i].baseCards[j].cardNumber;
                        leadingPlayer = tiedPlayers[i];
                    }
                    else if (tiedPlayers[i].baseCards[j].cardNumber == biggestNumber && leadingPlayer != tiedPlayers[i]) leadingPlayer = null;
                }
            }
        }
        else
        {
            //Create func, which iteraters through all cards from highest to lowest, continue for ties.
            Dictionary<PlayerScript, List<Card>> cardsToCheck = new Dictionary<PlayerScript, List<Card>>();

            for(int i = 0; i < tiedPlayers.Count; i++)
            {
                List<Card> highestPlayerCards = new List<Card>();
                cardsToCheck.Add(tiedPlayers[i], highestPlayerCards);
            }


            for (int i = 0; i < tiedPlayers.Count; i++)
            {



                int biggest = BiggestAmongSameNumbers(tableCards, tiedPlayers[i]);
                if (biggest > biggestNumber)
                {
                    biggestNumber = biggest;
                    leadingPlayer = tiedPlayers[i];
                }
                else if (biggest == biggestNumber) leadingPlayer = null;
            }
        }



        return leadingPlayer;
    }

    static PlayerScript ComparePair(List<Card> tableCards, PlayerScript currentWinner, PlayerScript player2)
    {

        //???


        return currentWinner;
    }




    static List<int> GetFiveHighestAscendingOrder(List<Card> cards, HandEnum hand)
    {
        List<int> toReturn = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            toReturn.Add(cards[i].cardNumber);
        }
        
        toReturn.Sort();
        toReturn.Remove(toReturn[0]);
        toReturn.Remove(toReturn[0]);


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
    static int BiggestAmongSameNumbers(List<Card> cards, PlayerScript player)
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
