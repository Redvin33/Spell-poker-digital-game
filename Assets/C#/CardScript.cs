using UnityEngine;

public class CardScript : MonoBehaviour
{
    public Card cardStats;
    SpriteRenderer spriteRenderer;
    bool hasEffect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasEffect = false;
    }
    public void AssignCard(Card card, Sprite baseSprite)
    {
        cardStats = card;
        spriteRenderer.sprite = baseSprite;
        if(card.cardEffect != null) hasEffect = true;
    }
}
