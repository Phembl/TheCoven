using System.Collections;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public int strength;
    public int repeatCount;
    public int targetID;
   
    protected virtual void Start()
    {
        
    }
    
    public abstract IEnumerator DoEffect(int boardID);

    public abstract string GetEffectText();

    protected string ConstructEffectTargetText(bool startSentence = false)
    {
        int randomAmount = repeatCount + 1;
        string effectTargetText = "";

        switch (targetID)
        {
            case 0: // Random
                if (randomAmount == 1) effectTargetText = startSentence ? "A random card in the arena" : "a random card in the arena";
                else effectTargetText = $"{randomAmount} random cards in the arena";
                break;
            
            case 1: //Self
                effectTargetText = startSentence ? "This card" : "this card";
                break;
            
            case 2: //Right
                effectTargetText = startSentence ? "The card to the right" : "the card to the right";
                break;
            
            case 3: //Left
                effectTargetText = startSentence ? "The card to the left" : "the card to the left";
                break;
            
            case 4: //All cards
                effectTargetText = startSentence ? "All cards in the arena" : "all cards in the arena";
                break;
            
            case 5: //Deck Random
                if (randomAmount == 1) effectTargetText = startSentence ? "A random card in the deck" : "a random card in the deck";
                else effectTargetText = $"{randomAmount} random cards in the deck";
                break;
            
            case 6: //Deck All
                effectTargetText = startSentence ? "All cards in the deck" : "all cards in the deck";
                break;
            
            case 7: //Hand random
                if (randomAmount == 1) effectTargetText = startSentence ? "A random card in hand" : "a random card in hand";
                else effectTargetText = $"{randomAmount} random cards in hand";
                break;
            
            case 8: //Hand All
                effectTargetText = startSentence ? "All cards in hand" : "all cards in hand";
                break;
               
            
            default:
                effectTargetText = "<color=red>PLACEHOLDER</color>";
                break;
        }

        return effectTargetText;
    }
}
