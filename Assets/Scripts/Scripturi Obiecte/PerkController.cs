using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkController : AbstractObiectController
{

    public override void collisionAction(Collision2D col)
    {
        Debug.Log("You just earned a perk,yay.");
    }

    protected override void StartMovement() { }
}
