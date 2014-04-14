using UnityEngine;
using System.Collections;

public class CatchingMiceCharacterAnimationMouse : CatchingMiceCharacterAnimation
{
    public override void OnHit()
    {
        if (currentAnimationClip != characterNameAnimation + _backAnimationClip + eatingAnimationClip)
        {
            //Debug.LogError("Loading Eating Animation Clip");
            PlayAnimation("UP/" + characterNameAnimation + _backAnimationClip + eatingAnimationClip);
            _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
        }
    }
    public override void PlayAnimation(string animationPath, bool moveRight = true)
    {
        string correctedAnimationPath = animationPath.Replace("LEFT", "RIGHT");
        base.PlayAnimation(correctedAnimationPath, !moveRight);
    }
    
}
