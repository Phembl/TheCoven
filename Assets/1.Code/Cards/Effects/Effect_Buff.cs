using UnityEngine;

public class Effect_Buff : Effect
{
    
    [SerializeField] public int buffAmount;
    [SerializeField] public int repeatAmount;
    [SerializeField] public int targetID;
    
    public override void DoEffect()
    {
        Debug.Log("Do effect");
    }

    public override string GetEffectText()
    {
        string effectText = "";
        string targetText = base.ConstructEffectTargetText(targetID, repeatAmount);

        effectText = $"Increase the power of {targetText} by <color=green>{buffAmount}</color>.";
        
        return effectText;
    }
}
