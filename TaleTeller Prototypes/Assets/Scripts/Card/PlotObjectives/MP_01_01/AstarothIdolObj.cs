using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 01:56 - Creation
/// </summary>
public class AstarothIdolObj : PlotObjective
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        //Subscribe UpdateStatus to junk death
        foreach (var junk in data.objective.linkedJunkedCards)
            junk.onCharDeath += UpdateStatus;
    }
}
