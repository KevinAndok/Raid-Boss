using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Area Around", menuName = "Spells/Damage Area Around", order = 1)]
public class DamageAreaAroundSpell : Spell
{
    [Header("Spell Specific")]
    public float distance;
    public float damagePerTick;

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
                c.TakeDamage(damagePerTick, 0);
                if (c.GetType() == typeof(Enemy)) ((Enemy)c).AddAggro(commandInfo.caster, 2);
            }

            yield return new WaitForSeconds(1);
            time++;

        } while (!commandInfo.caster.CommandHasChanged(commandInfo) && time < castTime);

        commandInfo.caster.commandCoroutine = null;
        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
