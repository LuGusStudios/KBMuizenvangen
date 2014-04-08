using UnityEngine;
using System.Collections;

public interface ICatchingMiceWorldObjectTrap
{
    int Stacks { get; set; }
    float Cost { get; set; }
    float Damage { get; set; }
    void OnHit(ICatchingMiceCharacter character);

    void DestroySelf();
}
