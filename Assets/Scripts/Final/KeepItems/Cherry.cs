using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : KeepUp
{
    public override void ApplyKeepUp(Ninja ninja)
    {
        ninja.KeepUp(1f);
    }
}
