using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal Area Around", menuName = "Spells/Heal Area Around", order = 1)]
public class HealAreaAroundSpell : Spell
{
    [Header("Spell Specific")]
    public float distance;
    public float healPerTick;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.commandCoroutine = commandInfo.caster.StartCoroutine(ExecuteCoroutine(commandInfo));
    }

    public IEnumerator ExecuteCoroutine(SpellCommand commandInfo)
    {
        int time = 0;

        commandInfo.position = commandInfo.caster.transform.position;

        do
        {
            List<Character> charactersHit = new List<Character>();

            foreach (Enemy e in Player.allEnemyCharacters) e.AddAggro(commandInfo.caster, 2);

            if (targetAllies)
                foreach (Hero h in Player.allHeroCharacters)
                    if (Vector2.Distance(commandInfo.caster.transform.position, h.transform.position) <= distance)
                        charactersHit.Add(h);
            if (targetEnemies)
                foreach (Enemy e in Player.allEnemyCharacters)
                    if (Vector2.Distance(commandInfo.caster.transform.position, e.transform.position) <= distance)
                        charactersHit.Add(e);

            foreach (Character c in charactersHit)
                c.RestoreHealth(healPerTick, 0);

            yield return new WaitForSeconds(1);
            time++;

        } while (!commandInfo.caster.CommandHasChanged(commandInfo) && time < castTime);

        commandInfo.caster.commandCoroutine = null;
        if (commandInfo.caster.CommandList.Count > 0 && commandInfo.caster.CommandList[0] == commandInfo) commandInfo.caster.CommandList.RemoveAt(0);
    }
}
