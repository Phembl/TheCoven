using System.Collections;
using UnityEngine;

public class Effect_CardDraw : Effect
{
    
    
    public override IEnumerator DoEffect(int boardID)
    {
        int drawCount = strength - 1;
        
        for (int i = 0; i <= drawCount; i++)
        {
            HandManager.instance.DrawCards(1);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public override string GetEffectText()
    {
        int drawAmount = strength;
        
        string effectText = "";

        effectText = $"Draw <color=green>{drawAmount}</color> cards.";
        
        return effectText;
    }
}
