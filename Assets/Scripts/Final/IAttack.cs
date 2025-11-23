using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    float checkInterval { get; set; }
    float timer { get; set; }


    void Attack();
}

