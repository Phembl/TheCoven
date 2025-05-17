using System.Collections;
using DG.Tweening;
using UnityEngine;
using VInspector;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;
    
    // Global consts
    private const float CARD_WIDTH = 400f;
    private const float HAND_CARDSPACING = 25f;
    private const int HAND_BASEORDER = 500;
    
    private BoxCollider2D handBounds;
    private float handWidth;
    
    // Editor Shown
    [Tab("References")]
    [SerializeField] private Transform handCardHolder;
    [SerializeField] private Transform deckCardHolder;
    [SerializeField] private GameObject deckButton;
    [EndTab] 
    
    [Header("Card draw Settings")]
    [SerializeField] private float drawDuration = 0.5f; // Duration of draw animation
    [SerializeField] private float delayBetweenDraws = 0.1f; // Delay between consecutive draws
    public int testCardsToDraw = 3;
    
    [Button("Draw Cards")]
    public void Button()
    {
        DrawCards(testCardsToDraw);
    }
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    void Start()
    {
        handBounds = GetComponent<BoxCollider2D>();
        handWidth = handBounds.size.x;
    }

    public void DrawCards(int cardsToDraw)
    {
        Debug.Log($"Drawing {cardsToDraw} Cards");
        StartCoroutine(DrawCardsToHand(cardsToDraw));
    }
    
    private IEnumerator DrawCardsToHand(int cardsToDraw)
    {
        
        // Get current number of cards in hand
        int cardsInHand = handCardHolder.childCount;

        // Make sure that not more cards can be drawn than are in deck
        int cardsLeftInDeck = deckCardHolder.childCount;
        if (cardsLeftInDeck < cardsToDraw)
        {
            cardsToDraw = cardsLeftInDeck;
        }
        
        // If no cards to draw, exit early
        if (cardsToDraw <= 0) yield break;

        // Calculate final positions for all cards (existing + new)
        float[] newPositions = CalculateCardPositions(cardsInHand + cardsToDraw);

        // If cards are already in hand, move existing cards to their new positions.
        if (cardsInHand > 0)
        {
            int lastChildIndex = cardsInHand - 1; //Get last child
            
            // Existing cards maintain their order but shift positions to make room
            for (int i = 0; i < cardsInHand; i++)
            {
                Debug.Log($"Pushing Card {lastChildIndex - i} to position {newPositions[i]}");
                Transform nextHandCard = handCardHolder.GetChild(0 + i); // Get next Handcard 
                float nextHandCardMoveX = newPositions[(0 + i)]; // Get the correct X from newPositions list
                Vector3 nextHandCardTarget = new Vector3(nextHandCardMoveX, nextHandCard.position.y, nextHandCard.position.z);
                
                nextHandCard.DOMove(nextHandCardTarget, drawDuration).SetEase(Ease.OutQuint); // Move Card
            }
        }


        // Draw new cards with slight delay between each
        for (int i = 0; i < cardsToDraw; i++)
        {
            // Pick random card from deck
            int nextCardNr = Random.Range(0, deckCardHolder.childCount);
            Transform nextCard = deckCardHolder.GetChild(nextCardNr);

            // Set parent to hand container
            nextCard.SetParent(handCardHolder);
            //nextCard.SetSiblingIndex(0);
            //nextCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = HAND_BASEORDER - 10 * i;
            UpdateHandOrder();

            // Move next card to deck button position to start animation
            nextCard.position = new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, nextCard.position.z);

            // For right-to-left drawing order of new cards:
            // First card (i=0) gets rightmost new position (cardsToDraw-1)
            // Last card gets leftmost new position (0)
            int newCardPositionIndex = cardsInHand + i;

            // Calculate target position
            float targetX = newPositions[newCardPositionIndex];
            Vector3 targetPosition = new Vector3(targetX, handBounds.offset.y, nextCard.position.z);

            // Animate the card to its position in hand
            nextCard.DOMove(targetPosition, drawDuration).SetEase(Ease.OutBack);
            

            // Wait before drawing next card for visual clarity
            yield return new WaitForSeconds(delayBetweenDraws);
        }
    }
    
    private float[] CalculateCardPositions(int totalCards)
    {
        float[] positions = new float[totalCards];
        
        // If no cards, return empty array
        if (totalCards == 0) return positions;
        
        // Calculate total width if using ideal spacing
        float idealTotalWidth = (totalCards * CARD_WIDTH) + ((totalCards - 1) * HAND_CARDSPACING);
        
        // Determine if we need to compress (overlap cards)
        bool needsCompression = idealTotalWidth > handWidth;
        
        // Calculate appropriate spacing based on available width
        float actualSpacing;
        if (needsCompression && totalCards > 1) {
            // Calculate compressed spacing (might be negative for overlap)
            actualSpacing = (handWidth - (totalCards * CARD_WIDTH)) / (totalCards - 1);
        } else {
            actualSpacing = HAND_CARDSPACING;
        }
        
        // Calculate total width with actual spacing
        float totalWidth = (totalCards * CARD_WIDTH) + ((totalCards - 1) * actualSpacing);
        
        // Calculate starting position (leftmost card)
        float startX = -totalWidth / 2 + (CARD_WIDTH / 2);
        
        // Calculate position for each card
        for (int i = 0; i < totalCards; i++) {
            positions[i] = startX + i * (CARD_WIDTH + actualSpacing);
            Debug.Log($"Calculated position{i}: {positions[i]}");
        }
        
        return positions;
    }
    
    public void UpdateHandPositions(bool animate = true)
    {
        // Get current number of cards in hand
        int cardsInHand = handCardHolder.childCount;
        
        // Calculate positions for all cards
        float[] cardPositions = CalculateCardPositions(cardsInHand);
        
        // Update position of each card
        for (int i = 0; i < cardsInHand; i++)
        {
           
            Transform nextCard = handCardHolder.GetChild(i);
            if (animate)
            {
                // Animate to new position
                nextCard.DOMoveX(cardPositions[i], drawDuration).SetEase(Ease.OutQuint);
            }
            else
            {
                // Instantly set new position
                Vector3 pos = nextCard.position;
                pos.x = cardPositions[i];
                nextCard.position = pos;
            }
        }
        UpdateHandOrder();
    } 
    

    private void UpdateHandOrder()
    {
        for (int i = 0; i < handCardHolder.childCount; i++)
        {
            
            Transform nextCard = handCardHolder.GetChild(i);
            nextCard.gameObject.name = "Card " + i;
            float nextCardZ = 0 + i;
            nextCard.transform.position = new Vector3(nextCard.transform.position.x, nextCard.transform.position.y, nextCardZ);
            nextCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = HAND_BASEORDER - i;
        }
    }
}
