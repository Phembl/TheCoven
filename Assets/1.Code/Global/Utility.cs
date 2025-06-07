using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.CardEffects;
using DG.Tweening;

namespace Game.Global
{
    public static class Utility
    { 
    
    //Constants
    public const float CARD_WIDTH = 400f;
    public const float HAND_CARDSPACING = 25f;
    public const float ARENA_CARDSPACING= 25f;
    public const int CARD_ORDER_BASE = 500;
    
    //Hand Settings
    private static BoxCollider2D handBounds => 
        BattleHandler.instance?.handBounds;
    
    //Arena Settings
    public static BoxCollider2D arenaBounds => 
        BattleHandler.instance?.arenaBounds;
      
      
 

#region ------------Hand Utils------------//
    
    public static IEnumerator DrawCardsToHand(int cardsToDraw)
    {
        Debug.Log($"Drawing {cardsToDraw} Cards");
        // Get current number of cards in hand
        int cardsInHand = Global.handCardHolder.childCount;

        // Make sure that not more cards can be drawn than are in deck
        int cardsLeftInDeck = Global.deckCardHolder.childCount;
        if (cardsLeftInDeck < cardsToDraw)
        {
            cardsToDraw = cardsLeftInDeck;
        }
        
        // If no cards to draw, exit early
        if (cardsToDraw <= 0) yield break;

        // Calculate final positions for all cards (existing + new)
        float[] newPositions = CalculateCardPositions(cardsInHand + cardsToDraw, handBounds.size.x, HAND_CARDSPACING);

        // If cards are already in hand, move existing cards to their new positions.
        if (cardsInHand > 0)
        {
            int lastChildIndex = cardsInHand - 1; //Get last child
            
            // Existing cards maintain their order but shift positions to make room
            for (int i = 0; i < cardsInHand; i++)
            {
                Transform nextHandCard = Global.handCardHolder.GetChild(0 + i); // Get next Handcard 
                float nextHandCardMoveX = newPositions[(0 + i)]; // Get the correct X from newPositions list
                Vector3 nextHandCardTarget = new Vector3(nextHandCardMoveX, nextHandCard.position.y, nextHandCard.position.z);
                
                //Nudge Card
                nextHandCard.DOMove(nextHandCardTarget, 0.2f * Global.timeMult).SetEase(Ease.OutQuint); // Move Card
            }
        }


        // Draw new cards with slight delay between each
        for (int i = 0; i < cardsToDraw; i++)
        {
            // Pick random card from deck
            int nextCardNr = Random.Range(0, Global.deckCardHolder.childCount);
            Transform nextCard = Global.deckCardHolder.GetChild(nextCardNr);

            // Set parent to hand container
            nextCard.SetParent(Global.handCardHolder);
            UpdateCardOrder(CardLocations.Hand, false);
            
            int newCardPositionIndex = cardsInHand + i;

            // Calculate target position
            float targetX = newPositions[newCardPositionIndex];
            Vector3 targetPosition = new Vector3(targetX, handBounds.offset.y, nextCard.position.z);
            
            // Move next card to draw Position under the final hand pos
            nextCard.position = new Vector3(targetPosition.x, targetPosition.y - 500, targetPosition.z);

            // Move the card to its position in hand
            yield return nextCard.GetComponent<Card>().MoveCard(targetPosition,CardLocations.Hand);

            BattleHandler.instance.UpdateCounter(BattleCounters.Deck, -1);
            
            // Wait before drawing next card for visual clarity
            yield return new WaitForSeconds(0.2f * Global.timeMult);
        }
        
        
    }
    
#endregion -------------Hand Utils----------//
    
#region ------------Card Utils------------//

    /// <summary>
    /// This calculates the duration how long the movement of a card takes from its current pos to a target pos.
    /// </summary>
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

        duration *= Global.timeMult;
        return duration;
    }
    
    /// <summary>
    /// This calculates the positions of all cards in a given area.
    /// It will automatically return overlapping positions if the area is not wide enough for all cards.
    /// </summary>
    public static float[] CalculateCardPositions(int totalCards, float cardAreaWidth, float cardSpacing)
    {
        float[] positions = new float[totalCards];
    
        // If no cards, return empty array
        if (totalCards == 0) return positions;
    
        // Calculate total width if using ideal spacing
        float idealTotalWidth = (totalCards * CARD_WIDTH) + ((totalCards - 1) * cardSpacing);
    
        // Determine if we need to compress (overlap cards)
        bool needsCompression = idealTotalWidth > cardAreaWidth;
    
        // Calculate appropriate spacing based on available width
        float actualSpacing;
        if (needsCompression && totalCards > 1) {
            // Calculate compressed spacing (might be negative for overlap)
            actualSpacing = (cardAreaWidth - (totalCards * CARD_WIDTH)) / (totalCards - 1);
        } else {
            actualSpacing = cardSpacing;
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
    
    /// <summary>
    /// This function sets the Z order and Sorting order of all cards in given container according to their sibling index.
    /// Optionally uses UpdateCardPositions when finished.
    /// </summary>
    public static void UpdateCardOrder(CardLocations location, bool updatePositions = true)
    {
    Transform cardHolder = null;
    
      switch (location)
      {
          case CardLocations.Hand:
              cardHolder = Global.handCardHolder;
              //areaWidth = handBounds.size.x;
              break;
            
          case CardLocations.Arena:
              cardHolder = Global.arenaCardHolder;
              //areaWidth = arenaBounds.size.x;
              break;
                
      }
        if (cardHolder == Global.arenaCardHolder) 
            Debug.Log("------STARTING Z ORDERING IN ARENA-----");
        
        if (cardHolder == Global.handCardHolder) 
            Debug.Log("------STARTING Z ORDERING IN HAND-----");
        
        for (int i = 0; i < cardHolder.childCount; i++)
        {
            Transform nextCard = cardHolder.GetChild(i);
            float nextCardZ = 0 + i;
            
            //nextCard.transform.localScale = new Vector3(1, 1, 1);
            
            if (cardHolder == Global.arenaCardHolder) 
                Debug.Log($"Next Card Z: {nextCardZ}");
            
            nextCard.transform.position = new Vector3
                (
                    nextCard.transform.position.x, 
                    nextCard.transform.position.y, 
                    nextCardZ
                );
            
            nextCard.transform.GetChild(0).
                GetComponent<Canvas>().sortingOrder = CARD_ORDER_BASE - i;
        }
        
        if (updatePositions) UpdateCardPositions(location);
    }
    
    private static void UpdateCardPositions(CardLocations location)
    {
        Transform cardHolder = null;
        float areaWidth = 0;
        
        switch (location)
        {
            case CardLocations.Hand:
                cardHolder = Global.handCardHolder;
                areaWidth = handBounds.size.x;
                break;
            
            case CardLocations.Arena:
                cardHolder = Global.arenaCardHolder;
                areaWidth = arenaBounds.size.x;
                break;
                
        }
        
        // Get current number of cards to Update
        int cardsToUpdate = cardHolder.childCount;
        
        // Calculate positions for all cards
        float[] cardPositions = CalculateCardPositions
            (cardsToUpdate, areaWidth, HAND_CARDSPACING);
        
        // Update position of each card
        int nextPosIndex = 0;
        foreach (Transform nextCard in cardHolder)
        {

            Vector3 nextPos = new Vector3(cardPositions[nextPosIndex], nextCard.transform.position.y, nextCard.transform.position.z);
            nextCard.DOMove
                (
                    nextPos, 
                    CalculateCardMoveDuration(nextPos, nextCard)
                )
                .SetEase(Ease.OutQuint);
            
            nextPosIndex++;
 
        }
    } 
    
#endregion ---------------Card Utils----------//
    
#region ------------Arena Utils------------//

/*
public static void AddCardToArena(GameObject card)
{
    //This is called from the card when it is dropped to the battlefield
    card.transform.SetParent(Global.arenaCardHolder);
        
    //Prepare Ray to check card order
    Vector2 rayOrigin = new Vector2((((arenaBounds.size.x + 100) / 2) * -1), arenaBounds.offset.y);
    Vector2 rayDirection = Vector2.right;
    int rayLayer = 1 << LayerMask.NameToLayer("Card");
        
    RaycastHit2D[] rayHits = Physics2D.RaycastAll(rayOrigin, rayDirection, 5000, rayLayer);
    
    //Set Card order according to physical order (because card could have been added left or right).
    int siblingIndex = 0;
    foreach (RaycastHit2D hit in rayHits)
    {
        hit.transform.SetSiblingIndex(siblingIndex);
        siblingIndex++;
    }
    
    UpdateCardPositions(CardLocations.Arena);
        
    }
    */




#endregion ------------Arena Utils----------//
    
#region ------------Exhaust Utils------------//

    public static void AddCardToExhaust(GameObject card)
    {
        card.transform.SetParent(Global.exhaustCardHolder);
        card.transform.position = new Vector3(3000, 0, 0);
        card.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
    }

#endregion ------------Exhaust Utils------------//  

    
    }//Class close
}//Namespace close


