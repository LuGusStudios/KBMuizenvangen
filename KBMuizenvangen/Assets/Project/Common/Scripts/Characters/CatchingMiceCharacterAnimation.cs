using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using System;

namespace KikaAndBob
{
    public enum MovementQuadrant
    {
        NONE = 0,

        UP = 1, // 0001
        RIGHT = 2, // 0010
        DOWN = 4, // 0100
        LEFT = 8, // 1000

        UP_RIGHT = 3, // north east : 0011
        DOWN_RIGHT = 6, // south east : 0110
        UP_LEFT = 9, // north west : 1001
        DOWN_LEFT = 12 // south west : 1100
    }
}

public class CatchingMiceCharacterAnimation : MonoBehaviour 
{
    public BoneAnimation[] animationContainers;
    public BoneAnimation currentAnimationContainer = null;

    public ICatchingMiceCharacter character = null;

    public string currentAnimationClip = "";
    public string currentAnimationPath = "";

    protected string _lastAnimationClip = "";

    //String example DOWN/Cat01Front_Jump --> 
    //"_currentMovementQuadrant  + / + characterNameAnimation + frontAnimationClip + jumpAnimationClip
    public string characterNameAnimation = "Cat01";
    public string jumpAnimationClip = "_Jump";
    public string walkAnimationClip = "_Walk";
    public string eatingAnimationClip = "_Attack";
    public string idleAnimationClip = "_Idle";

    protected string _sideAnimationClip = "Side";
    protected string _frontAnimationClip = "Front";
    protected string _backAnimationClip = "Back";

    protected KikaAndBob.MovementQuadrant _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
    protected bool _jumped = false;

    // convenience function
    protected KikaAndBob.MovementQuadrant DirectionToQuadrant(Vector3 movementDirection)
    {

        // 1. Figure out the quadrant for the movementDirection
        int quadrant = (int)KikaAndBob.MovementQuadrant.NONE;

        // movementDirection.x indicates left or right (and so also the sign of the localScale.x)
        // movementDirection.y indicates up or down

        // factor in epsilons (ex. going up is not exactly 0.0f, but between [-1.0f, and 1.0f])

        if (movementDirection.x < -0.2f)
        {
            quadrant = quadrant | (int)KikaAndBob.MovementQuadrant.LEFT;
        }
        else if (movementDirection.x > 0.2f)
        {
            quadrant = quadrant | (int)KikaAndBob.MovementQuadrant.RIGHT;
        }

        if (movementDirection.y > 0.3f)
        {
            quadrant = quadrant | (int)KikaAndBob.MovementQuadrant.UP;
        }
        else if (movementDirection.y < -0.3f)
        {
            quadrant = quadrant | (int)KikaAndBob.MovementQuadrant.DOWN;
        }

        KikaAndBob.MovementQuadrant quadrantReal = (KikaAndBob.MovementQuadrant)Enum.ToObject(typeof(KikaAndBob.MovementQuadrant), quadrant);

        if (quadrantReal == KikaAndBob.MovementQuadrant.NONE)
        {
            //Debug.LogError(name + ": quadrant was NONE " + quadrant + "/" + movementDirection + " : defaulting to RIGHT");
            quadrantReal = KikaAndBob.MovementQuadrant.RIGHT;
        }

        return quadrantReal;
    }

	// Use this for initialization
	void Start () 
    {
	 
	}
	
	// Update is called once per frame
	void Update () 
    {
        AnimationLoop();
	}

    protected virtual void AnimationLoop()
    {
        if (!character.moving && !character.attacking)
        {
            IdleLoop();
            return;
        }

        Vector3 movementDirection = character.movementDirection;
        
        KikaAndBob.MovementQuadrant newQuadrant = DirectionToQuadrant(movementDirection);
        
        if ((character.moving) && (newQuadrant != _currentMovementQuadrant) && (!character.jumping)) 
        {
            LoadQuadrantAnimation(newQuadrant,walkAnimationClip);
        }
    }

    protected virtual void IdleLoop()
    {
        if (currentAnimationClip != characterNameAnimation + _frontAnimationClip + idleAnimationClip)
        {
            //Debug.LogError("Loading Idle Animation Clip");
            PlayAnimation("DOWN/" + characterNameAnimation + _frontAnimationClip + idleAnimationClip);
            _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE; 
        }
           
    }
    protected void LoadQuadrantAnimation(KikaAndBob.MovementQuadrant quadrantReal, string animationType)
    {
        // 1. Map the quadrant to the correct AnimationClip name
        // 2. Find the AnimationClip object and Play() it

        _currentMovementQuadrant = quadrantReal;

        // 1. Map the quadrant to the correct AnimationClip name

        // we only have animations for the left side of the quadrants (up, left_up, left, left_down and down)
        // the other side is achieved by mirroring the animations by changing localScale.x
        KikaAndBob.MovementQuadrant quadrantAnimation = quadrantReal;
        bool movingRight = false;
        if ((quadrantReal & KikaAndBob.MovementQuadrant.RIGHT) == KikaAndBob.MovementQuadrant.RIGHT)
        {
            movingRight = true;

            // use bitwise NOT operator to remove RIGHT, then add LEFT with OR operator 
            // http://stackoverflow.com/questions/750240/how-do-i-set-or-clear-the-first-3-bits-using-bitwise-operations
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.RIGHT);
            quadrantAnimation = quadrantAnimation | KikaAndBob.MovementQuadrant.LEFT;
        }

        // we have no LEFT_UP and LEFT_DOWN here
        // so just use the LEFT for the diagonal movement by removing both UP and DOWN
        if ((quadrantAnimation & KikaAndBob.MovementQuadrant.LEFT) == KikaAndBob.MovementQuadrant.LEFT)
        {
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.UP);
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.DOWN);
        }

        string animationClipName = "" + quadrantAnimation.ToString() + "/" + characterNameAnimation;

        animationClipName += CheckFacing(quadrantAnimation) + animationType;
        

        PlayAnimation(animationClipName, !movingRight);
    }
    public virtual void PlayAnimation(string animationPath, bool moveRight = true)
    {

        string[] parts = animationPath.Split('/');
        if (parts.Length != 2)
        {
            Debug.LogError(name + " : AnimationPath should be a string with a single / as separator! " + animationPath);
            return;
        }

        string containerName = parts[0];
        string clipName = parts[1];
		

        currentAnimationContainer = null;
        foreach (BoneAnimation container in animationContainers)
        {
            if (container.name == containerName)
            {
                currentAnimationContainer = container;
                currentAnimationContainer.gameObject.SetActive(true);
                currentAnimationContainer.animation.enabled = true;
            }
            else
            {
                container.gameObject.SetActive(false);
                //currentAnimationContainer.animation.enabled = false;
            }
        }

        if (currentAnimationContainer == null)
        {
            Debug.LogError(name + " : No animation found for name " + animationPath);
            currentAnimationContainer = animationContainers[0];
        }

        currentAnimationPath = animationPath;
        currentAnimationClip = clipName;

        
        //Debug.Log("PLAYING ANIMATION " + clipName + " ON " + currentAnimationContainer.name);
        if (currentAnimationClip.Contains("Jump"))
        {
            currentAnimationContainer.Stop();
            currentAnimationContainer.Play(clipName);
            //Debug.LogError("PLAYING CLIP " + clipName);
        }
        else
        {
            currentAnimationContainer.CrossFade(clipName,0.3f); 
        }


        if (moveRight)
        {
            // if going right, the scale.x needs to be positive 
            if (currentAnimationContainer.transform.localScale.x < 0)
            {
                currentAnimationContainer.transform.localScale = currentAnimationContainer.transform.localScale.x(Mathf.Abs(currentAnimationContainer.transform.localScale.x));
            }
        }
        else // moving left
        {
            // if going left, the scale.x needs to be negative
            if (currentAnimationContainer.transform.localScale.x > 0)
            {
                currentAnimationContainer.transform.localScale = currentAnimationContainer.transform.localScale.x(currentAnimationContainer.transform.localScale.x * -1.0f);
            }
        }
    }

    

    protected String CheckFacing(KikaAndBob.MovementQuadrant quadrantAnimation)
    {
        string facing = "";
        switch (quadrantAnimation)
        {
            case KikaAndBob.MovementQuadrant.UP:
                facing = _backAnimationClip;
                break;
            case KikaAndBob.MovementQuadrant.RIGHT:
                //because of the naming of cat no definition of Side needs to be added
                if (character is CatchingMiceCharacterPlayer && !character.jumping)
                {
                    break;
                }
                facing = _sideAnimationClip;
                break;
            case KikaAndBob.MovementQuadrant.DOWN:
                facing = _frontAnimationClip;
                break;
            case KikaAndBob.MovementQuadrant.LEFT:
                if (character is CatchingMiceCharacterPlayer && !character.jumping)
                {
                    break;
                }
                facing =_sideAnimationClip;
                break;
            default:
                Debug.LogError("no correct movement quadrant had been chosen");
                break;
        }
        return facing;
    }
    public virtual void SetupLocal()
    {
        if (animationContainers.Length == 0)
        {
            animationContainers = transform.GetComponentsInChildren<BoneAnimation>();
        }

        if (animationContainers.Length == 0)
        {
            Debug.LogError(name + " : no BoneAnimations found for this animator!");
        }

        SetCharacter();

        //PlayAnimation("DOWN/" + characterNameAnimation + _frontAnimationClip + idleAnimationClip);
    }
    public void OnJump()
    {
        KikaAndBob.MovementQuadrant quadrant = DirectionToQuadrant(character.movementDirection);
        LoadQuadrantAnimation(quadrant,jumpAnimationClip);
        _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE; 
    }
    protected virtual void SetCharacter()
    {
        if (character == null)
        {
            character = transform.GetComponent<ICatchingMiceCharacter>();
        }

        if (character == null)
        {
            Debug.LogError(name + " : no character found!");
        }
        else
        {
            character.onJump += OnJump;
            character.onHit += OnHit;
        }
    }
    public virtual void OnHit()
    {

        if (currentAnimationClip != characterNameAnimation + _frontAnimationClip + eatingAnimationClip)
        {
            Debug.LogError("Loading Idle Animation Clip");
            PlayAnimation("DOWN/" + characterNameAnimation + _frontAnimationClip + eatingAnimationClip);
            _currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
        }
        
    }

    protected void Awake()
    {
        SetupLocal();
    }
    protected void OnDisable()
    {
        character.onJump -= OnJump;
        character.onHit -= OnHit;
    }
}
