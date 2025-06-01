using System.Collections;
using UnityEngine;
using Game.CardEffects;
using DG.Tweening;

namespace Game.Global
{
    public enum CardLocations
    {
        None,
        Deck,
        Hand,
        Arena,
        Exhaust
    }
    
    public static class Utility
    { 
    //Constants
    private const float CARD_WIDTH = 400f;
    private const float HAND_CARDSPACING = 25f;
    private const int HAND_ORDER_BASE = 500;
        
    //Card Container
    private static Transform handCardHolder = BattleManager.instance.handCardHolder;
    private static Transform deckCardHolder = BattleManager.instance.deckCardHolder;
    private static Transform arenaCardHolder = BattleManager.instance.arenaCardHolder;
    
    //Hand Settings
    private static Transform deckIcon = BattleManager.instance.deckIcon;
    private static float cardDrawDuration = BattleManager.instance.drawDuration;
    private static float cardDrawDelayBetweenCards = BattleManager.instance.delayBetweenDraws;
    private static BoxCollider2D handBounds = BattleManager.instance.handBounds;
      
#region ------------Card Effect Utils------------//
        
    /// <summary>
    /// This function takes a CardEffectTarget and the siblingID of the effect user in the Arena.
    /// It returns a target GameObject.
    /// </summary>
    public static GameObject GetCardEffectTarget(CardEffectTargets effectTarget, int resolverBoardID)
    {
        GameObject targetCard = null;

        int randomTarget;

        switch (effectTarget)
        {
            case CardEffectTargets.Random:
                randomTarget = Random.Range(0, arenaCardHolder.childCount);
                targetCard = arenaCardHolder.GetChild(randomTarget).gameObject;
                break;
            case CardEffectTargets.Self:
                targetCard = arenaCardHolder.GetChild(resolverBoardID).gameObject;
                break;
            case CardEffectTargets.Right:
                if (resolverBoardID + 1 == arenaCardHolder.childCount) break; //Checks if the resolved card is not the most-right
                targetCard = arenaCardHolder.GetChild(resolverBoardID + 1).gameObject;
                break;
        }
        return targetCard;
    } 
#endregion ------------Card Effect Utils------------//  

#region ------------Hand Utils------------//
    
    public static IEnumerator DrawCardsToHand(int cardsToDraw)
    {
        Debug.Log($"Drawing {cardsToDraw} Cards");
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
        float[] newPositions = CalculateHandCardPositions(cardsInHand + cardsToDraw);

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
                
                nextHandCard.DOMove(nextHandCardTarget, cardDrawDelayBetweenCards).SetEase(Ease.OutQuint); // Move Card
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
            nextCard.position = new Vector3(deckIcon.position.x, deckIcon.position.y, nextCard.position.z);

            // For right-to-left drawing order of new cards:
            // First card (i=0) gets rightmost new position (cardsToDraw-1)
            // Last card gets leftmost new position (0)
            int newCardPositionIndex = cardsInHand + i;

            // Calculate target position
            float targetX = newPositions[newCardPositionIndex];
            Vector3 targetPosition = new Vector3(targetX, handBounds.offset.y, nextCard.position.z);

            // Animate the card to its position in hand
            nextCard.DOMove(targetPosition, cardDrawDuration).SetEase(Ease.OutBack);
            

            // Wait before drawing next card for visual clarity
            yield return new WaitForSeconds(cardDrawDelayBetweenCards);
        }
    }
    
    private static float[] CalculateHandCardPositions(int totalCards)
    {
        float[] positions = new float[totalCards];
    
        // If no cards, return empty array
        if (totalCards == 0) return positions;
    
        // Calculate total width if using ideal spacing
        float idealTotalWidth = (totalCards * CARD_WIDTH) + ((totalCards - 1) * HAND_CARDSPACING);
    
        // Determine if we need to compress (overlap cards)
        bool needsCompression = idealTotalWidth > handBounds.size.x;
    
        // Calculate appropriate spacing based on available width
        float actualSpacing;
        if (needsCompression && totalCards > 1) {
            // Calculate compressed spacing (might be negative for overlap)
            actualSpacing = (handBounds.size.x - (totalCards * CARD_WIDTH)) / (totalCards - 1);
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
            //Debug.Log($"Calculated position{i}: {positions[i]}");
        }
    
        return positions;
    }
    
    public static void UpdateHandPositions(bool animate = true)
    {
        // Get current number of cards in hand
        int cardsInHand = handCardHolder.childCount;
        
        // Calculate positions for all cards
        float[] cardPositions = CalculateHandCardPositions(cardsInHand);
        
        // Update position of each card
        for (int i = 0; i < cardsInHand; i++)
        {
           
            Transform nextCard = handCardHolder.GetChild(i);
            if (animate)
            {
                // Animate to new position
                nextCard.DOMoveX(cardPositions[i], cardDrawDelayBetweenCards).SetEase(Ease.OutQuint);
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
    
    private static void UpdateHandOrder()
    {
        for (int i = 0; i < handCardHolder.childCount; i++)
        {
            
            Transform nextCard = handCardHolder.GetChild(i);
            nextCard.gameObject.name = "Card " + i;
            float nextCardZ = 0 + i;
            nextCard.transform.position = new Vector3(nextCard.transform.position.x, nextCard.transform.position.y, nextCardZ);
            nextCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = HAND_ORDER_BASE - i;
        }
    }
#endregion -------------Card Effect Utils----------//
    
#region ------------Card Utils------------//
    public static float CalculateCardMoveDuration(Vector3 targetPos, Transform card)
    {
        // Get the distance between current position and target position
        float distance = Vector3.Distance(card.position, targetPos);

        // Define our min and max distances for interpolation
        float minDistance = 10f;    
        float maxDistance = 2000f;   

        // Define our min and max durations
        float minDuration = 0.05f;
        float maxDuration = 0.5f;

        // Clamp the distance to our defined range
        float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate the normalized value (0 to 1) based on the distance
        float normalizedValue = (clampedDistance - minDistance) / (maxDistance - minDistance);

        // Interpolate between min and max duration
        float duration = Mathf.Lerp(minDuration, maxDuration, normalizedValue);

        return duration;
    }
#endregion ---------------Card Utils----------//
    
    
    
    
    
    
    }//Class close
}//Namespace close


