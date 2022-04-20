using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Lower Defense", menuName = "Spells/Lower Defense", order = 1)]
public class LowerDefenseSpell : Spell
{
    [Header("Spell Specific")]
    [Range(0, 1)] public float amount;
    public float duration;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.target.StartCoroutine(LowerDefense(commandInfo));

        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator LowerDefense(SpellCommand commandInfo)
    {
        commandInfo.target.stats.resistance -= amount;
        yield return new WaitForSeconds(duration);
        commandInfo.target.stats.resistance += amount;
    }
}
