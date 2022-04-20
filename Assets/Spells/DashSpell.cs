using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Spells/Dash", order = 1)]
public class DashSpell : Spell
{
    [Header("Spell Specific")]
    public float jumpTime;
    public float jumpSpeed;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.commandCoroutine = commandInfo.caster.StartCoroutine(ExecuteCoroutine(commandInfo));
    }

    public IEnumerator ExecuteCoroutine(SpellCommand commandInfo)
    {
        Transform character = commandInfo.caster.transform;
        float time = 0;

        float speed = jumpSpeed * Time.fixedDeltaTime;
        var direction = (commandInfo.position - commandInfo.caster.position).normalized;

        commandInfo.position = commandInfo.caster.position + direction * jumpSpeed * jumpTime;

        do
        {
            yield return new WaitForFixedUpdate();

            character.position += (Vector3)direction * speed;

            time += Time.fixedDeltaTime;
        } while (time < jumpTime);

        commandInfo.caster.commandCoroutine = null;
        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
