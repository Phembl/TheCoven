using UnityEngine;

public class Effect_CardDraw : Effect
{
    
    [SerializeField] public int drawAmount;
    
    public override void DoEffect()
    {
        Debug.Log("Do effect");
    }

    public override string GetEffectText()
    {
        string effectText = "";

        effectText = $"Draw <color=green>{drawAmount}</color> cards.";
        
        return effectText;
    }
}
