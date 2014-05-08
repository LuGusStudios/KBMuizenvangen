using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceMouseSkinny : CatchingMiceCharacterMouse 
{
    public delegate void OnAttack();
    public event OnAttack onAttack;
    
    public override void GetTarget()
    {
        if (CatchingMiceLevelManager.use.TrapTiles.Count <= 0)
        {
            //no traps left, check for cheese
            Debug.Log("No traps has been found, checking for cheese.");
            base.GetTarget();
            return;
        }

        //search for traps first before checking for cheese
        List<CatchingMiceTile> tiles = new List<CatchingMiceTile>(CatchingMiceLevelManager.use.TrapTiles);
        targetWaypoint = GetTargetWaypoint(tiles);
            
        if (targetWaypoint != null)
        {
                
            //Debug.LogError("Getting new trap " + CatchingMiceLevelManager.use.trapTiles.Count);
            CalculateTarget(targetWaypoint); 
        }
        else
        {
            Debug.LogError("No target found");
            //try go for cheese instead
            base.GetTarget();
        }
        
        
    }
    public override void DoCurrentTileBehaviour(int pathIndex)
    {
        base.DoCurrentTileBehaviour(pathIndex);
        if ((currentTile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap && pathIndex==0)
        {
            //handle.StopRoutine();
            //begin eating the cheese
            StartCoroutine(AttackTrap());
            //handle = LugusCoroutines.use.StartRoutine(AttackTrap()); 
        }
    }
    public IEnumerator AttackTrap()
    {
        attacking = true;
        if (onAttack != null)
            onAttack();
        CatchingMiceTile trapTile = currentTile;
        while (_health > 0 && trapTile.trapObject != null && trapTile.trapObject.Stacks > 0)
        {
            //Debug.Log(currentTile.trapObject.Stacks);
            trapTile.trapObject.Health -= damage;

            yield return new WaitForSeconds(attackInterval);
        }
        attacking = false;
      

        GetTarget();
        
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        CatchingMiceLevelManager.use.TrapRemoved += TargetRemoved;

    }
    protected override void OnDisable()
    {
        //base.OnDisable();
        //CatchingMiceLevelManager.use.TrapRemoved -= TargetRemoved;
    }
    public override void DieRoutine()
    {
        base.DieRoutine();
        CatchingMiceLevelManager.use.TrapRemoved -= TargetRemoved;
    }
}
