using TMPro;
using UnityEngine;

public class PlayerUIDEBUG : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text mana;

    public TableCardScript[] tableCardScripts;
    int cards;
    public void AssignPlayer(string pname, int pmana)
    {
        name.text = pname;
        mana.text = pmana.ToString();
        cards = 0;
    }

    public void AssignCards(Card card)
    {      
        tableCardScripts[cards].DealCard(card.cardNumber, card.cardSuit);
        cards++;
    }

    public void ResetCards()
    {
        for(int i = 0; i < tableCardScripts.Length;i++)
        {
            tableCardScripts[i].Refresh();
        }
        cards = 0;
    }
}
