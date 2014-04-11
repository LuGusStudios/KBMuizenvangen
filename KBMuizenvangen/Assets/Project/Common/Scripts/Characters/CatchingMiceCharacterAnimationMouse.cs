using UnityEngine;
using System.Collections;

public class CatchingMiceCharacterAnimationMouse : CatchingMiceCharacterAnimation
{

    protected override void IdleLoop()
    {
        // idle of the mice will be eating, no normal idle state
        if (currentAnimationClip != characterNameAnimation + _sideAnimationClip + idleAnimationClip)
        {
            PlayAnimation("RIGHT/" + characterNameAnimation + _sideAnimationClip + idleAnimationClip);
        }
    }
    public override void PlayAnimation(string animationPath, bool moveRight = true)
    {
        string correctedAnimationPath = animationPath.Replace("LEFT", "RIGHT");
        base.PlayAnimation(correctedAnimationPath, !moveRight);
    }
    public override void SetupLocal()
    {
        base.SetupLocal();
        PlayAnimation("DOWN/" + characterNameAnimation + _frontAnimationClip + walkAnimationClip);
    }
}
