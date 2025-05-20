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

            ArenaManager.instance.AttackButtonIsPressed();
        }
    }
}
