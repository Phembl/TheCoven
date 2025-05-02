using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager instance;
    
    [Tab("References")]
    [SerializeField] private GameObject battlefieldCardHolder;
    [EndTab] 
    
    // Global consts
    private const float BATTLEFIELD_CARDSPACE = 25f;
    private const float CARD_WIDTH = 400f;
    
    private BoxCollider2D battlefieldBounds;
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
 
    void Start()
    {
        battlefieldBounds = GetComponent<BoxCollider2D>();
    }
    
    // This is called by the cards to find their position on the battlefield when dropped
    public Vector3 GetBattlefieldPosition(int direction)
    {
        float battlefieldYOffset = battlefieldBounds.offset.y;
        float cardXOffset = CARD_WIDTH + BATTLEFIELD_CARDSPACE;
        
        Vector3 newPosition = new Vector3(0, battlefieldYOffset, 0);
        
        int childCount = battlefieldCardHolder.transform.childCount;
        List<float> cardXPosition = new List<float>();
        
        if  (childCount > 0)
        {
            foreach (Transform child in battlefieldCardHolder.transform)
            {
                float nextCardX = child.transform.position.x;
                cardXPosition.Add(nextCardX);
            }

            if (direction > 0)
            {
                float newCardX = cardXPosition.Max() + cardXOffset;
                newPosition.x = newCardX;
                
            }
            else
            {
                float newCardX = cardXPosition.Min() - cardXOffset;
                newPosition.x = newCardX;
            }
        }
        
        return newPosition;
    }

    public void NewCardOnBattlefield(GameObject card)
    {
        card.transform.SetParent(battlefieldCardHolder.transform);
    }
}
