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
        moving
    }
    [ShowInInspector, ReadOnly]
    private CardLocations currentLocation = CardLocations.Deck;
    [ShowInInspector, ReadOnly]
    private CardStates currentState = CardStates.resting;
    
    private int originalSortingOrder;
    private Coroutine resetOrderCoroutine;
    
    //Object variables
    private Canvas cardCanvas;
    private GraphicRaycaster raycaster;
    private Camera cam;


    void Awake()
    {
        cardCanvas = transform.GetChild(0).GetComponent<Canvas>();
        cam = Camera.main;
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
        
        if (hoverScaleTween != null) transform.DOKill(true);
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
        if (currentLocation == CardLocations.Arena) return;
        
        // Move card with mouse while maintaining offset
        transform.position = GetMouseWorldPosition() + dragOffset;
    }

    void OnMouseUp()
    {   
        if(currentState != CardStates.dragged) return;
        if (currentLocation == CardLocations.Arena) return;
        
        if (hoverScaleTween != null) hoverScaleTween.Kill();
        if (transform.localScale.x != 1) hoverScaleTween = transform.DOScale(1f, scaleTweenTime).SetEase(Ease.OutQuad);

        
        // Check for Arena Hit
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, arenalayerMask);
        
        if (rayHit)
        {
            Vector3 nextCardPos = new Vector3(0,0,0);
            
            if (Input.mousePosition.x < Screen.width / 2) //Battlefield left
            {
                Debug.Log("Dropping Card to Battlefield left");
                nextCardPos = Utility.GetArenaCardPosition(-1);
            }
            else //Battlefield right
            {
                Debug.Log("Dropping Card to Battlefield right");
                nextCardPos = Utility.GetArenaCardPosition(1);
            }
            
            MoveCard(nextCardPos, CardLocations.Arena);
            

        }
        else
        {
            MoveCard(startPosition, CardLocations.Hand);
        }
        
        
    }
    
    #endregion -------------Card Mouse Interactions----------//
    
    #region ------------Card Animations------------//
    
    public IEnumerator AnimateCard(CardAnimations animation)
    {
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
        }
        
        
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
    
    
    #endregion ------------Card Animations------------//
    
    #region ------------Card Helper------------//
    
    public void MoveCard(Vector3 targetPosition, CardLocations targetLocation, bool fromDrag = true)
    {
        currentState = CardStates.moving;
       
        transform.DOMove(
                targetPosition, 
                Utility.CalculateCardMoveDuration(targetPosition, transform) * Global.timeMult)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                currentState = CardStates.resting;
                currentLocation = targetLocation;
                
                switch (targetLocation)
                {
                    case CardLocations.Hand: 
                        if (fromDrag) cardCanvas.sortingOrder = originalSortingOrder;
                        break;
                    
                    case CardLocations.Arena: 
                        //cardCanvas.sortingOrder = originalSortingOrder;
                        Utility.AddCardToArena(gameObject);
                        Utility.UpdateCardPositions(CardLocations.Hand);
                        break;
                    
                    case CardLocations.Exhaust:
                        Utility.AddCardToExhaust(gameObject);
                        break;
                }
               
            });
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
