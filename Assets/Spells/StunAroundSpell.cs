using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Around", menuName = "Spells/Stun Around", order = 1)]
public class StunAroundSpell : Spell
{
    [Header("Spell Specific")]
    public float distance;
    public float duration;
    public float damage;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.position = commandInfo.caster.transform.position;

        List<Character> charactersHit = new List<Character>();

        WaitCommand command = new WaitCommand
        {
            type = CommandTypes.Wait,
            waitTime = duration,
            canBeCancelled = false,
            position = commandInfo.position
        };

        if (targetAllies)
            foreach (Hero h in Player.allHeroCharacters)
                if (Vector2.Distance(commandInfo.position, h.position) <= distance)
                    if (h != commandInfo.caster)
                        charactersHit.Add(h);
        if (targetEnemies)
            foreach (Enemy e in Player.allEnemyCharacters)
                if (Vector2.Distance(commandInfo.position, e.position) <= distance)
                    charactersHit.Add(e);

        foreach (Character c in charactersHit)
        {
            c.TakeDamage(damage, 0);
            if (c.GetType() == typeof(Enemy)) ((Enemy)c).AddAggro(commandInfo.caster, 2);
            if (c.CommandList[0].canBeCancelled) c.CommandList.Insert(0, command); //apply stun
        }

        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
