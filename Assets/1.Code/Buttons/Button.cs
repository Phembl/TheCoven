using System;
using System.Collections;
using UnityEngine;
using Game.Global;
using DG.Tweening;

public class Button : MonoBehaviour
{

    private bool isInactive;
    private Tweener colorTweenFrame;
    private Tweener colorTweenIcon;
    private Color originalColor;
    
    private Vector3 animationVector; 
    
    public Color hoverColor; 

    public SpriteRenderer frameRenderer;
    public SpriteRenderer iconRenderer;

    public ButtonTypes type;

    void Start()
    {
        animationVector = new Vector3(0,-8,0);
        originalColor = frameRenderer.color;
    }

    private void OnMouseOver()
    {
        if (!isCurrentlyUsable()) return;
        
        if (colorTweenFrame != null)
            
        {
            colorTweenFrame.Kill();
            colorTweenIcon.Kill();
        }
        
        colorTweenIcon = iconRenderer.DOColor(hoverColor, 0.2f);
        colorTweenIcon = frameRenderer.DOColor(hoverColor, 0.2f);
    }

    void OnMouseDown()
    {
        if (!isCurrentlyUsable()) return;

        RunClickAnimation();

        StartCoroutine(CheckButton());
    }

    void OnMouseExit()
    {
        if (colorTweenFrame != null)
            
        {
            colorTweenFrame.Kill();
            colorTweenIcon.Kill();
        }
        
        colorTweenIcon = iconRenderer.DOColor(originalColor, 0.2f);
        colorTweenIcon = frameRenderer.DOColor(originalColor, 0.2f);
    }

    IEnumerator CheckButton()
    {
        switch (type)
        {
            case ButtonTypes.Resolve:

                if (BattleHandler.instance.arenaCardHolder.childCount > 0)
                {
                    BattleHandler.instance.ResolveButtonIsPressed();
                }

                break;

            case ButtonTypes.Redraw:
                if (BattleHandler.instance.isCurrentlyResolving == false)
                {
                    isInactive = true;
                    yield return StartCoroutine(Utility.DrawCardsToHand(1));
                    isInactive = false;
                }

                break;
        }
    }
    
    void RunClickAnimation()
    {
        transform.DOPunchPosition(animationVector, 0.15f);
            
    }

    bool isCurrentlyUsable()
    {
        bool isUsable = true;

        if (isInactive) return false;

        if (BattleHandler.instance.isCurrentlyResolving) return false;
        
        return isUsable;
    }
}
