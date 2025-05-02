using System.Collections.Generic;
using DG.Tweening;
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
    private float scaleTweenTime = 0.2f;
    private float scalePercent = 1.2f;
    private Tween hoverScaleTween;
    
    //Dragging Variables
    private Vector3 startPosition;
    private Vector3 dragOffset;
    
    //State tracking variable
    private enum Places
    {
        inDeck,
        inHand,
        onBattlefield,
        inExhaust
    }
    private enum States
    {
        resting,
        hovering,
        dragged,
        moving
    }
    [ShowInInspector, ReadOnly]
    private Places currentPlace = Places.inDeck;
    [ShowInInspector, ReadOnly]
    private States currentState = States.resting;
    
    private int originalSortingOrder;
    
    //Object variables
    private Canvas cardCanvas;
    private GraphicRaycaster raycaster;
    private Camera cam;


    protected virtual void Start()
    {
        cardCanvas = transform.GetChild(0).GetComponent<Canvas>();
        cam = Camera.main;
    }
    
    void OnMouseEnter()
    {
        if(currentState != States.resting) return;
        
        currentState = States.hovering;
        
        originalSortingOrder = cardCanvas.sortingOrder;
        cardCanvas.sortingOrder = ORDER_HOVERING;
        
        if (hoverScaleTween != null) transform.DOKill(true);
        hoverScaleTween = transform.DOScale(scalePercent, scaleTweenTime).SetEase(Ease.OutQuad);
    }

    void OnMouseExit()
    {
        if(currentState != States.hovering) return;

        currentState = States.resting;
        
        cardCanvas.sortingOrder = originalSortingOrder;
        
        if (hoverScaleTween != null) transform.DOKill(true);
        hoverScaleTween = transform.DOScale(1, scaleTweenTime).SetEase(Ease.OutQuad);
    }
 
    void OnMouseDown()
    {
        if(currentState != States.hovering) return;
        if (currentPlace == Places.onBattlefield) return;
        currentState = States.dragged;
        
        startPosition = transform.position;
        cardCanvas.sortingOrder = ORDER_DRAGGING;
        
        // Calculate and store offset between mouse position and card position
        dragOffset = transform.position - GetMouseWorldPosition();
        
    }
    
    void OnMouseDrag() 
    {
        if(currentState != States.dragged) return;
        if (currentPlace == Places.onBattlefield) return;
        
        // Move card with mouse while maintaining offset
        transform.position = GetMouseWorldPosition() + dragOffset;
    }

    void OnMouseUp()
    {   
        if(currentState != States.dragged) return;
        if (currentPlace == Places.onBattlefield) return;
        
        // Check for Battlefield
        // Create a layerMask that ONLY includes the Battlefield layer
        int battlefieldLayer = LayerMask.NameToLayer("Battlefield");
        int layerMask = (1 << battlefieldLayer);
        
        // Create a ray from the camera through the mouse position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
        
        
        // Check if the raycast hit
        if (rayHit)
        {
            Vector3 nextCardPos = new Vector3(0,0,0);
            
            if (Input.mousePosition.x < Screen.width / 2) //Battlefield left
            {
                Debug.Log("Dropping Card to Battlefield left");
                nextCardPos = BattlefieldManager.instance.GetBattlefieldPosition(-1);
            }
            else //Battlefield right
            {
                Debug.Log("Dropping Card to Battlefield right");
                nextCardPos = BattlefieldManager.instance.GetBattlefieldPosition(1);
            }
            
            MoveCard(nextCardPos, 1);
            
        }
        else
        {
            MoveCard(startPosition, 0);
        }
        
        
    }

    public void MoveCard(Vector3 targetPosition, int targetPlace)
    {
        // Targetplace 0 = toHand, 1 = toBattlefield
        currentState = States.moving;
        
        if (hoverScaleTween != null) transform.DOKill(true);
        if (transform.localScale.x != 1) transform.DOScale(1f, scaleTweenTime).SetEase(Ease.OutQuad);
        
        transform.DOMove(targetPosition, CalculateMoveDuration(targetPosition)).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                currentState = States.resting;
                
                switch (targetPlace)
                {
                    case 0: // ToHand
                        currentPlace = Places.inHand;
                        
                        cardCanvas.sortingOrder = originalSortingOrder;
                        break;
                    
                    case 1: // ToBattlefield
                        currentPlace = Places.onBattlefield;
                        
                        cardCanvas.sortingOrder = originalSortingOrder;
                        BattlefieldManager.instance.NewCardOnBattlefield(gameObject);
                        break;
                }
               
            });
    }
    
    //Helper Functions
    private Vector3 GetMouseWorldPosition() 
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        return mousePos;
    }
    
    public float CalculateMoveDuration(Vector3 targetPos)
    {
        // Get the distance between current position and target position
        float distance = Vector3.Distance(transform.position, targetPos);
    
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
    
}
