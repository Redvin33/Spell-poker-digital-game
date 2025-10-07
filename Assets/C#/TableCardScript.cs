using TMPro;
using UnityEngine;

public class TableCardScript : MonoBehaviour
{
    public int cardID;
    public TMP_Text numberText;
    public TMP_Text suitText;

    private void Start()
    {
        Refresh();
    }


    public void DealCard(int number, SuitEnum suit)
    {
        numberText.text = number.ToString();
        suitText.text = suit.ToString();
        suitText.color = SuitColor.GetSuitColor(suit);
        this.gameObject.SetActive(true);
    }

    public void Refresh() => this.gameObject.SetActive(false);
}
