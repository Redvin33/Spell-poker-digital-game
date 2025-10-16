using System.Collections.Generic;
using UnityEngine;

public class TwoPairHelper
{
    public PlayerScript player;
    public List<int> pairs;
    public int leftOver;
    public Dictionary<int, int> cardPerNumber;
    public TwoPairHelper(PlayerScript player)
    {
        this.player = player;
        pairs = new List<int>();
        cardPerNumber = new Dictionary<int, int>();
        leftOver = 0;
    }

    public void CheckPairs()
    {
        foreach (var keys in cardPerNumber.Keys)
        {
            if (cardPerNumber[keys] == 2) pairs.Add(keys);
            else if (cardPerNumber[keys] == 1 && keys > leftOver) leftOver = keys;
        }
        pairs.Sort();
        pairs.Reverse();
    } 

    public int GetPairIteration(int iteration)
    {
        if (iteration <= pairs.Count) return pairs[iteration];
        else return 0;

    }
}
