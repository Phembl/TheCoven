using UnityEngine;

public class Button_Resolve : MonoBehaviour
{

    private bool isInactive;


    void OnMouseDown()
    {
        if 
            (BattleHandler.instance.isCurrentlyResolving == false &&
             BattleHandler.instance.arenaCardHolder.childCount > 0)
        {
            BattleHandler.instance.ResolveButtonIsPressed();
        }
    }
}
