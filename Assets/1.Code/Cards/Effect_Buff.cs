using UnityEngine;

public class Effect_Buff : Effect
{
    
    private enum CardTargets
    {
        Random,
        Self,
        Next,
        Last,
        All,
        DeckRandom,
        DeckAll,
        HandRandom,
        HandAll
    }
    [SerializeField] public int buffAmount;
    [SerializeField] public int randomAmount;
    [SerializeField] public int targetID;
    
    public override void DoEffect()
    {
        Debug.Log("Do effect");
    }

    public override string GetEffectText()
    {
        string effectText = "";
        string targetText = base.ConstructEffectTargetText(targetID, randomAmount);

        effectText = $"Buff {targetText} by <color=green>{buffAmount}</color>";
        
        return effectText;
    }
}
