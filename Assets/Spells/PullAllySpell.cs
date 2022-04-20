using System.Collections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Pull Ally", menuName = "Spells/Pull(Ally)", order = 1)]
public class PullAllySpell : Spell
{
    [Header("Spell Specific")]
    public float speed;
    [Range(0, 1)] public float distanceBetweenPulledObject;

    public override void Execute(SpellCommand commandInfo)
    {
        if (commandInfo.target.GetType() == typeof(Hero))
        {
            commandInfo.caster.StartCoroutine(Pull(commandInfo));
        }
        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator Pull(SpellCommand commandInfo)
    {
        Vector2 destination;

        do
        {
            destination = commandInfo.caster.position + ((commandInfo.caster.size + commandInfo.target.size + distanceBetweenPulledObject) * (commandInfo.target.position - commandInfo.caster.position).normalized);

            commandInfo.target.transform.position = Vector2.MoveTowards(commandInfo.target.position, destination, speed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();

        } while (commandInfo.target.transform.position != (Vector3)destination);

    }
}
