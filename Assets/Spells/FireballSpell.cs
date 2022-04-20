using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "Spells/Fireball", order = 1)]
public class FireballSpell : Spell
{
    [Header("Spell Specific")]
    public float damage;
    public float speed;
    public GameObject fireballPrefab;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.commandCoroutine = commandInfo.caster.StartCoroutine(CastFireball(commandInfo));
    }

    public IEnumerator CastFireball(SpellCommand commandInfo)
    {
        float time = 0;

        do
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        } while (!commandInfo.caster.CommandHasChanged(commandInfo) && time < castTime);

        Vector2 direction = (commandInfo.target.position - commandInfo.caster.position).normalized;
        Vector2 spawnPosition = commandInfo.caster.position + ((commandInfo.caster.size + .55f) * direction);

        var fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity).GetComponent<FireballController>();

        fireball.target = commandInfo.target;
        fireball.damage = damage;
        fireball.speed = speed;

        ((Enemy)commandInfo.target).AddAggro(commandInfo.caster, 2);

        commandInfo.caster.commandCoroutine = null;
        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
