using UnityEngine;
using System.Collections;

public class CatchingMiceTrapInteractive : ICatchingMiceTrapType 
{
    public GameObject miniTrap = null;

    public override void SetupLocal()
    {
        base.SetupLocal();

        _startTime = CatchingMiceGameManager.use.Timer - interval;
    }

    public override void DoBehaviour()
    {
        if (CatchingMiceGameManager.use.Timer - _startTime < interval)
            return;

        _trap.Stacks--;
        //give traps a timeout before you can use it again
        ResetTimer();

        Debug.LogError("placing mini traps");

        //spits out "mini traps" around itself
        CatchingMiceTile[] tiles = CatchingMiceLevelManager.use.GetTileAround(_trap.TrapObject.parentTile);

        foreach (CatchingMiceTile tile in tiles)
        {
            if (tile == null)
                continue;
            //only add on floortiles
            if (tile.worldObject != null)
                continue;
            //when there is no trap spawn a minitrap
            if(tile.trapObject == null)
            {
                GameObject trapPrefab = (GameObject)Instantiate(miniTrap);
                ICatchingMiceWorldObjectTrap trap = null;
                trap = trapPrefab.GetComponent(typeof(ICatchingMiceWorldObjectTrap)) as ICatchingMiceWorldObjectTrap;
                trap.TrapObject.parentTile = tile;
                trap.TrapObject.transform.position = tile.location;
            }
        }
    }
}
