using System.Collections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Pull Enemy", menuName = "Spells/Pull(Enemy)", order = 1)]
public class PullEnemySpell : Spell
{
    [Header("Spell Specific")]
    public float speed;
    public bool canPullBosses;
    [Range(0, 1)] public float distanceBetweenPulledObject;

    public override void Execute(SpellCommand commandInfo)
    {
        if (commandInfo.target.GetType() == typeof(Enemy) && !(!canPullBosses && Player.instance.bosses.Contains(commandInfo.target))) 
            commandInfo.caster.StartCoroutine(Pull(commandInfo));

        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator Pull(SpellCommand commandInfo)
    {
        float desiredDistance = commandInfo.caster.size + commandInfo.target.size + distanceBetweenPulledObject;

        do
        {
            commandInfo.target.transform.position = Vector2.MoveTowards(commandInfo.target.position, commandInfo.caster.transform.position, speed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();

        } while (Vector2.Distance(commandInfo.caster.transform.position, commandInfo.target.transform.position) >= desiredDistance);

        ((Enemy)commandInfo.target).AddAggro(commandInfo.caster, 1);
    }
}
