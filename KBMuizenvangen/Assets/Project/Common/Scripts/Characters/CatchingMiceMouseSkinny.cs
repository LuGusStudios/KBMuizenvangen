using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceMouseSkinny : CatchingMiceCharacterMouse 
{
    public delegate void OnAttack();
    public event OnAttack onAttack;
    
    public override void GetTarget()
    {
        //search for traps first before checking for cheese
        if (CatchingMiceLevelManager.use.trapTiles.Count > 0)
        {
            List<CatchingMiceTile> tiles = new List<CatchingMiceTile>(CatchingMiceLevelManager.use.trapTiles);
            targetWaypoint = GetTargetWaypoint(tiles);
            
            if (targetWaypoint != null)
            {
                Debug.LogError("Getting new trap " + CatchingMiceLevelManager.use.trapTiles.Count);
                CalculateTarget(targetWaypoint); 
            }
            else
            {
                Debug.LogError("No target found");
            }
        }
        else
        {
            //no traps left, check for cheese
            Debug.Log("No traps has been found, checking for cheese.");
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
            trapTile.trapObject.Stacks -= (int)damage;

            yield return new WaitForSeconds(attackInterval);
        }
        attacking = false;
      


        GetTarget();
        
    }
    protected override void OnEnable()
    {
        CatchingMiceLevelManager.use.CheeseRemoved += TargetRemoved;
        CatchingMiceLevelManager.use.TrapRemoved += TargetRemoved;

    }
    public override void DieRoutine()
    {
        base.DieRoutine();
        CatchingMiceLevelManager.use.TrapRemoved -= TargetRemoved;
    }
}
