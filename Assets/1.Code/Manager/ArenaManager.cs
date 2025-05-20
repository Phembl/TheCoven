using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;
using DG.Tweening;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager instance;
    [HideInInspector] public float speedMult = 1;
    
    [Tab("References")]
    [SerializeField] private GameObject battlefieldCardHolder;

    [EndTab]

    //Speed variables
    private float animationScaleSpeed = 0.3f;
    
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
        animationScaleSpeed *= speedMult;
        
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
        //This is called from the card when it is dropped to the battlefield
        card.transform.SetParent(battlefieldCardHolder.transform);
        
        //Prepare Ray to check card order
        Vector2 rayOrigin = new Vector2((((battlefieldBounds.size.x + 100) / 2) * -1), battlefieldBounds.offset.y);
        Vector2 rayDirection = Vector2.right;
        int rayLayer = 1 << LayerMask.NameToLayer("Card");
        
        Debug.Log(rayOrigin);
        
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(rayOrigin, rayDirection, 5000, rayLayer);
        
        //Set Card Sibling order to match Battlefield order
        int siblingIndex = 0;
        foreach (RaycastHit2D hit in rayHits)
        {
            hit.transform.SetSiblingIndex(siblingIndex);
            siblingIndex++;
        }
        
    }

    public void AttackButtonIsPressed()
    {
        StartCoroutine(ResolveBoard());
    }
    
    //This resolves the Board, taking one child after another and calling the card effects
    private IEnumerator ResolveBoard() 
    {
        Debug.Log("Resolving board");

        foreach (Transform nextCard in battlefieldCardHolder.transform)
        {
            //Prepare Card
            Canvas nextCardCanvas = nextCard.GetChild(0).GetComponent<Canvas>();
            int nextCardOriginalSortingOrder = nextCardCanvas.sortingOrder;
            
            //Animate Card
            nextCardCanvas.sortingOrder += 1000;
            nextCard.DOScale(1.2f, animationScaleSpeed);
            yield return new WaitForSeconds(animationScaleSpeed + 0.2f);
            
            Effect nextCardEffect = nextCard.GetComponent<Effect>();
            
            if (nextCardEffect != null)
            {
                int boardID = nextCard.transform.GetSiblingIndex();
                yield return StartCoroutine(nextCardEffect.DoEffect(boardID));
            }
            else
            {
                Debug.Log("No effect");
            }
            
            yield return new WaitForSeconds(1f);
            
            //Finish Up Card
            nextCardCanvas.sortingOrder = nextCardOriginalSortingOrder;
            nextCard.DOScale(1f, animationScaleSpeed);
            yield return new WaitForSeconds(animationScaleSpeed);
        }
    }
    
}


















































