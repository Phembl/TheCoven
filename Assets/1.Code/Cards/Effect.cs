using UnityEngine;

public abstract class Effect : MonoBehaviour
{
   
    protected virtual void Start()
    {
        
    }
    
    public abstract void  DoEffect();

    public abstract string GetEffectText();

    protected string ConstructEffectTargetText(int targetID, int randomAmount = 0)
    {
        string effectTargetText = "";

        switch (targetID)
        {
            case 2: //Next
                effectTargetText = "the next card";
                break;
            default:
                effectTargetText = "THIS NEEDS TO BE FILLED IN";
                break;
        }

        return effectTargetText;
    }
}
