using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Summon Pet", menuName = "Spells/Summon Pet", order = 1)]
public class SummonPetSpell : Spell
{
    [Header("Spell Specific")]
    public GameObject petPrefab;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.commandCoroutine = commandInfo.caster.StartCoroutine(ExecuteCoroutine(commandInfo));
    }

    public IEnumerator ExecuteCoroutine(SpellCommand commandInfo)
    {
        float time = 0;

        var caster = commandInfo.caster;
        var target = commandInfo.target;

        do
        {
            yield return new WaitForFixedUpdate();

            time += Time.fixedDeltaTime;
        } while (time < castTime);

        Vector2 spawnPosition = caster.transform.position + (target.transform.position - caster.transform.position).normalized * 2 * caster.size;

        var pet = Instantiate(petPrefab, spawnPosition, Quaternion.identity).GetComponent<Hero>();
        pet.CreateCommand(new AttackCommand
        {
            type = CommandTypes.Attack,
            target = target,
            position = target.position
        });

        if (commandInfo.target.GetType() == typeof(Enemy)) ((Enemy)commandInfo.target).AddAggro(commandInfo.caster, 2);

        caster.commandCoroutine = null;
        caster.CommandList.RemoveAt(0);
    }
}
