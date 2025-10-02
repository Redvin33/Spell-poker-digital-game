using UnityEngine;

public class Card
{
    //Create child script?
    public SuitEnum cardSuit;
    public int cardNumber;
    public CardEffect cardEffect;
    public Card(SuitEnum cardSuit, int cardNumber, CardEffect cardEffect)
    {
        this.cardSuit = cardSuit;
        baseSuit = cardSuit;
        this.cardNumber = cardNumber;
        baseNumber = cardNumber;
        if(cardEffect != null) this.cardEffect = cardEffect;
    }


    //Base stats (Reset always)
    SuitEnum baseSuit;
    int baseNumber;
    public void ResetCard()
    {
        cardSuit = baseSuit;
        cardNumber = baseNumber;
    }
}
