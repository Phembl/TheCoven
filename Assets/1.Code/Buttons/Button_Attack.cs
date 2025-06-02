using UnityEngine;

public class Button_Attack : MonoBehaviour
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
