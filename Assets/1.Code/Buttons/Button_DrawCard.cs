using System.Collections;
using UnityEngine;
using Game.Global;

public class Button_DrawCard : MonoBehaviour
{

    private bool isInactive;
    
    void OnMouseDown()
    {
        if (!isInactive && BattleHandler.instance.isCurrentlyResolving == false)
        {
            isInactive = true;

            StartCoroutine(DrawCard());
        }
    }

    IEnumerator DrawCard()
    {
        Debug.Log("Drawing Card Button Pressed");
        yield return StartCoroutine(Utility.DrawCardsToHand(1));
        isInactive = false;
    }
}
