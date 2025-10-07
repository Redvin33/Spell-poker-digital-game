using UnityEngine;

public static class SuitColor
{

    public static Color GetSuitColor(SuitEnum suit)
    {
        if (suit == SuitEnum.Nature) return Color.green;
        else if (suit == SuitEnum.Elemental) return Color.cyan;
        else if (suit == SuitEnum.Time) return Color.black;
        else return Color.red;
    }
}
