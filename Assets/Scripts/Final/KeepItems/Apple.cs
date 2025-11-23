using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : KeepUp
{
    public override void ApplyKeepUp(Ninja ninja)
    {
        ninja.KeepUp(1);
    }
}
