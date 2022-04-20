using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public class AggroPoints
    {
        public Hero character;
        public float points;
    }

    public List<AggroPoints> aggroPoints = new List<AggroPoints>();

    public AggroPoints target = null;

    new void Update()
    {
        base.Update();

        if (!IsDead)
        {
            var highestAggro = GetHighestAggro();

            if (highestAggro != null) target = highestAggro;

            if (target != null)
            {
                if (!target.character.IsDead && (CommandList.Count == 0 || (CommandList.Count > 0 && CommandList[0].GetType() == typeof(AttackCommand) && ((AttackCommand)CommandList[0]).target != target.character)))
                {
                    if (commandCoroutine != null)
                    {
                        StopCoroutine(commandCoroutine);
                        commandCoroutine = null;
                    }

                    CreateCommand(new AttackCommand
                    {
                        type = CommandTypes.Attack,
                        target = target.character,
                        position = target.character.transform.position
                    });
                }
            }
        }
        else
        {
            if (CommandList.Count > 0)
            {
                CommandList.RemoveAt(0);
                commandCoroutine = null;
            }
        }
    }

    public AggroPoints GetHighestAggro()
    {
        if (aggroPoints.Count == 0) return null;

        AggroPoints aggro = null;

        for (int i = 0; i < aggroPoints.Count; i++)
        {
            if (aggroPoints[i].character.IsDead) continue;
            if (aggro == null) aggro = aggroPoints[i];
            else if (aggro.points < aggroPoints[i].points) aggro = aggroPoints[i];
        }

        return aggro;
    }

    public void AddAggro(Hero hero, float aggro)
    {
        foreach (AggroPoints ap in aggroPoints)
        {
            if (ap.character == hero)
            {
                ap.points += aggro;
                return;
            }
        }

        aggroPoints.Add(new AggroPoints { character = hero, points = aggro });
    }
}
