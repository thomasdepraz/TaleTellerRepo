using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Rémi Sécher - 12/08/2021 22:51 - Creation
/// </summary>
public class TheophilusThePriestObj : PlotObjective
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        foreach(var junk in data.objective.linkedJunkedCards)
        {
            junk.onCharDeath += UpdateStatus;
        }
    }
}
