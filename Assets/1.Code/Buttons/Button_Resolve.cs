using UnityEngine;

public class Button_Resolve : MonoBehaviour
{

    private bool isInactive;
    void Start()
    {
        
    }
    

    void OnMouseDown()
    {
        if (!isInactive)
        {
            isInactive = true;

            BattleManager.instance.ResolveButtonIsPressed();
        }
    }
}
