using UnityEngine;
using System.Collections;

public interface ICatchingMiceWorldObjectTrap
{
    float Health { get; set; }
    int Stacks { get; set; }
    float Cost { get; set; }
    float Damage { get; set; }

    CatchingMiceWorldObject TrapObject { get; }
    void OnHit(ICatchingMiceCharacter character);
    void DestroySelf();
    void DoBehaviour();
}
