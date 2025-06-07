using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Global;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;


public class Card : MonoBehaviour

{
    
    //Order consts
    private const int ORDER_HOVERING = 1000;
    private const int ORDER_DRAGGING = 1500;
    
    //Hovering Variables
    private float scaleTweenTime = 0.3f;
    private float scalePercent = 1.2f;
    private Tween hoverScaleTween;
    
    //Dragging Variables
    private Vector3 startPosition;
    private Vector3 dragOffset;
    
    int arenalayerMask = (1 << 6);
    private int currentSortingOrder;
    
    //State tracking variable
    private enum CardStates
    {
        resting,
        hovering,
        dragged,
        moving,
        resolving,
        animating
    }
    [ShowInInspector, ReadOnly]
    private CardLocations currentLocation = CardLocations.Deck;
    [ShowInInspector, ReadOnly]
    private CardStates currentState = CardStates.resting;
    
    private int originalSortingOrder;
    private Coroutine resetOrderCoroutine;
    private Tween cardMove;
    
    //Object variables
    private Canvas cardCanvas;
    private GraphicRaycaster raycaster;
    private Camera cam;


    void Awake()
    {
        cardCanvas = transform.GetChild(0).GetComponent<Canvas>();
        cam = Camera.main;
        
        float waitAfterEffect = 0.2f * Global.timeMult;
    }
    
    #region ------------Card Mouse Interactions------------//
    void OnMouseEnter()
    {
        if(currentState != CardStates.resting) return;
        
        currentState = CardStates.hovering;
        
        if(resetOrderCoroutine == null) originalSortingOrder = cardCanvas.sortingOrder;
        else StopCoroutine(resetOrderCoroutine);

        cardCanvas.sortingOrder = ORDER_HOVERING;

        if (hoverScaleTween != null) hoverScaleTween.Kill();
        
        hoverScaleTween = transform.DOScale(scalePercent, scaleTweenTime).SetEase(Ease.OutQuad);
    }

    void OnMouseExit()
    {
        if(currentState != CardStates.hovering) return;

        currentState = CardStates.resting;
        
        //cardCanvas.sortingOrder = originalSortingOrder;
        cardCanvas.sortingOrder = ORDER_HOVERING - (transform.GetSiblingIndex() + 1);
        
        if (hoverScaleTween != null) hoverScaleTween.Kill();
        
        hoverScaleTween = transform.DOScale(1, scaleTweenTime).SetEase(Ease.OutQuad);
        resetOrderCoroutine = StartCoroutine(ResetOrder(scaleTweenTime));

    }
 
    void OnMouseDown()
    {
        if(currentState != CardStates.hovering) return;
        if (currentLocation == CardLocations.Arena) return;
        currentState = CardStates.dragged;
        
        startPosition = transform.position;
        cardCanvas.sortingOrder = ORDER_DRAGGING;
        
        // Calculate and store offset between mouse position and card position
        dragOffset = transform.position - GetMouseWorldPosition();
        
    }
    
    void OnMouseDrag() 
    {
        if(currentState != CardStates.dragged) return;
        if (currentLocation != CardLocations.Hand) return;
        
        // Move card with mouse while maintaining offset
        transform.position = GetMouseWorldPosition() + dragOffset;
    }

    void OnMouseUp()
    {   
        if(currentState != CardStates.dragged) return;
        if (currentLocation != CardLocations.Hand) return;
        
        if (hoverScaleTween != null) hoverScaleTween.Kill();
        if (transform.localScale.x != 1) hoverScaleTween = transform.DOScale(1f, scaleTweenTime).SetEase(Ease.OutQuad);

        
        // Check for Arena Hit
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, arenalayerMask);
        
        if (rayHit)
        {
            StartCoroutine(DropCardToArena());
        }
        else
        {
            StartCoroutine(DropCardBackToHand());
        }
        
        
    }
    
    #endregion -------------Card Mouse Interactions----------//
    
    #region ------------Card Animations------------//
    
    public IEnumerator AnimateCard(CardAnimations animation)
    {
        currentState = CardStates.animating;
        
        switch (animation)
        {
            case CardAnimations.Attack:
                yield return StartCoroutine(AnimateCardAttack());
                break;
            
            case CardAnimations.UpdatePower:
                yield return StartCoroutine(AnimateCardUpdatePower());
                break;
            
            case CardAnimations.ResolveStart:
                yield return StartCoroutine(AnimateCardResolveStart());
                break;
            
            case CardAnimations.ResolveEnd:
                yield return StartCoroutine(AnimateCardResolveEnd());
                break;
            
            case CardAnimations.Exhaust:
                yield return StartCoroutine(AnimateCardExhaust());
                break;
            
            case CardAnimations.Appear:
                yield return StartCoroutine(AnimateCardAppear());
                break;
        }
        
        currentState = CardStates.resting;
        
        
    }

    private IEnumerator AnimateCardAttack()
    {
        //Attack Anim
        float animationTime = 0.2f * Global.timeMult;
        Vector3 moveVector = new Vector3(0,70,0);
        transform.DOPunchPosition(moveVector, animationTime);
        yield return new WaitForSeconds(animationTime);
    }
    
    private IEnumerator AnimateCardUpdatePower()
    {
        float animationTime = 0.2f * Global.timeMult;
        transform.DOShakeRotation(animationTime, new Vector3(0, 0, 10));
        yield return new WaitForSeconds(animationTime);
    }

    private IEnumerator AnimateCardResolveStart()
    {
        float animationTime = 0.2f * Global.timeMult;
        
        currentSortingOrder = cardCanvas.sortingOrder;
        cardCanvas.sortingOrder += 1000;
        
        transform.DOScale(1.2f, animationTime);
        yield return new WaitForSeconds(animationTime);
    }

    private IEnumerator AnimateCardResolveEnd()
    {
        float animationTime = 0.2f * Global.timeMult;
        cardCanvas.sortingOrder = currentSortingOrder;
        
        transform.DOScale(1f, animationTime);
        yield return new WaitForSeconds(animationTime);
    }

    private IEnumerator AnimateCardExhaust()
    {
        Debug.Log("Card Animation Exhaust");
        float animationTime = 0.4f * Global.timeMult;
        
        cardCanvas.GetComponent<CanvasGroup>().DOFade(0, animationTime);
        yield return new WaitForSeconds(animationTime);
        
       
    }
    
    private IEnumerator AnimateCardAppear()
    {
        float animationTime = 0.4f * Global.timeMult;
        
        cardCanvas.GetComponent<CanvasGroup>().DOFade(1, animationTime);
        yield return new WaitForSeconds(animationTime);
       
    }


    #endregion ------------Card Animations------------//
    
    #region ------------Card Helper------------//

    public IEnumerator ResolveCardEffects()
    {
        // Get all Effect components and resolves them
        Effect[] nextCardEffects = GetComponents<Effect>();
        if (nextCardEffects.Length > 0)
        {
            currentState = CardStates.resolving;
            //Animate Card
            yield return StartCoroutine
                (AnimateCard(CardAnimations.ResolveStart));
            
            currentState = CardStates.resolving;
            
            int boardID = transform.GetSiblingIndex();
            foreach (Effect effect in nextCardEffects)
            {
                yield return StartCoroutine(effect.DoEffect(boardID));
                yield return new WaitForSeconds(0.2f * Global.timeMult);
            }
            
            //Finish Up Card
            yield return StartCoroutine
                (AnimateCard(CardAnimations.ResolveEnd));
            
            currentState = CardStates.resting;
        }
    }
    
    
    private IEnumerator DropCardBackToHand()
    {
        yield return StartCoroutine (MoveCard(startPosition, CardLocations.Hand));
        cardCanvas.sortingOrder = originalSortingOrder;
        
    }
    
    private IEnumerator DropCardToArena()
    {
        //This is the center of the arena
        Vector3 nextCardPos = new Vector3(0,Utility.arenaBounds.offset.y,0);
        int nextCardSiblingIndex = 0;
        
        //If this is the first card to drop use middle pos
        if (Global.arenaCardHolder.childCount != 0)
        {
            float[] arenaPositions = Utility.CalculateCardPositions
            (
                Global.arenaCardHolder.childCount + 2,
                Utility.arenaBounds.size.x,
                Utility.ARENA_CARDSPACING
            );
                
            if (Input.mousePosition.x < Screen.width / 2) //Battlefield left
            {
                Debug.Log("Dropping Card to Battlefield left");
                nextCardPos.x = arenaPositions[0];
                nextCardSiblingIndex = 0;
            }
            else //Battlefield right
            {
                Debug.Log("Dropping Card to Battlefield right");
                nextCardPos.x = arenaPositions[^1];
                nextCardSiblingIndex = arenaPositions.Length - 1;
            }
        }
            
        transform.SetParent(Global.arenaCardHolder);
        transform.SetSiblingIndex(nextCardSiblingIndex);
        
        //Move Card to the Arena
        yield return StartCoroutine
            (MoveCard(nextCardPos, CardLocations.Arena));
        
        if (cardMove != null) cardMove.Kill();
        
        yield return new WaitForSeconds(0.1f);
        Utility.UpdateCardOrder(CardLocations.Arena);
        Utility.UpdateCardOrder(CardLocations.Hand);
        
        yield return new WaitForSeconds(0.5f * Global.timeMult);
        
        StartCoroutine(ResolveCardEffects());
    }
    
    public IEnumerator MoveCard
        (
            Vector3 targetPosition, 
            CardLocations targetLocation,
            bool instant = false
        )
    {
        currentState = CardStates.moving;
        
        //Set Card Move Time
        float cardMoveTime = 0f;
        if (!instant) 
            cardMoveTime = Utility.CalculateCardMoveDuration(targetPosition, transform);

        if (cardMove != null) cardMove.Kill();
        cardMove = transform.DOMove(targetPosition, cardMoveTime).SetEase(Ease.OutQuad);
            
        yield return new WaitForSeconds(cardMoveTime);
            
        currentState = CardStates.resting;
        currentLocation = targetLocation;
    }
    private Vector3 GetMouseWorldPosition() 
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        return mousePos;
    }
    
    private IEnumerator ResetOrder(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cardCanvas.sortingOrder = originalSortingOrder;
    }
    
    #endregion ------------Card Helper------------//
    
}
