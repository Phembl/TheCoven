using System.Collections;
using UnityEngine;

public class Effect_Buff : Effect
{
/*
    public override string GetEffectText()
    {
        string effectText = "";
        int buffAmount = strength;
        
        //Calls the base class to construct target text 
        string targetText = ConstructEffectTargetText();

        effectText = $"Increase the power of {targetText} by <color=green>{buffAmount}</color>.";
        
        return effectText;
    }
    
    public override IEnumerator DoEffect(int boardID)
    {
        int buffAmount = strength;
        //BoardID is the sibling number of this card in the arena
        Debug.Log("Do Buff effect");
        for (int i = 0; i <= repeatCount; i++)
        {
            GameObject cardTarget = BattleManager.instance.GetCardTarget(targetID, boardID);
            cardTarget.GetComponent<Character>().UpdatePower(buffAmount);
            yield return new WaitForSeconds(0.2f);
        }
 
        yield return null;
    }
    */
}
