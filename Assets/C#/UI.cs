using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Setup")]
    public GameObject debugStartButtons;
    public GameObject nextRoundButton;
    public GameStateManager gameStateManager;


    [Header("PlayerUIDEBUG")]
    public PlayerUIDEBUG[] playerUIDebug;


    public void EnableStartButtons(bool enable) => debugStartButtons.SetActive(enable);
    public void DefaultStart()
    {
        EnableStartButtons(false);
        gameStateManager.DefaultStart();
    }

    public void DebugStart()
    {
        EnableStartButtons(false);
        gameStateManager.DebugStart();
    }

    public void EnableNextRoundButton(bool enable) => nextRoundButton.SetActive(enable);
    public void NextRoundButton()
    {
        RefreshUICards();
        EnableNextRoundButton(false);
        gameStateManager.ResetTurn();
    }

    public void AssignPlayer(PlayerScript player, int id)
    {
        playerUIDebug[id].AssignPlayer(player.playerName, player.currentMana);
    }
    public void AssignCards(int id, Card card)
    {
        playerUIDebug[id].AssignCards(card);
    }
    public void RefreshUICards()
    {
        for(int i = 0; i < playerUIDebug.Length; i++)
        {
            playerUIDebug[i].ResetCards();       
        }
    }
}
