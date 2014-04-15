using UnityEngine;
using System.Collections;

public class CatchingMiceAnimationMouseSkinny : CatchingMiceCharacterAnimationMouse 
{
    public string attackAnimationClip = "_Attack";
    public override void OnHit()
    {
        if (currentAnimationClip != characterNameAnimation + _sideAnimationClip + eatingAnimationClip)
        {
            //Debug.LogError("Loading Idle Animation Clip");
            PlayAnimation("RIGHT/" + characterNameAnimation + _sideAnimationClip + eatingAnimationClip);
            _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
        }
    }
    protected void OnAttack()
    {
        if (currentAnimationClip != characterNameAnimation + _sideAnimationClip + attackAnimationClip)
        {
            //Debug.LogError("Loading attack Animation Clip");
            PlayAnimation("RIGHT/" + characterNameAnimation + _sideAnimationClip + attackAnimationClip);
            _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
        }
    }
    protected override void SetCharacter()
    {
        if (character == null)
        {
            character = transform.GetComponent<CatchingMiceMouseSkinny>();
        }

        if (character == null)
        {
            Debug.LogError(name + " : no character found!");
        }
        else
        {
            character.onJump += OnJump;
            character.onHit += OnHit;
            ((CatchingMiceMouseSkinny)character).onAttack += OnAttack;
        }
    }
    
}
