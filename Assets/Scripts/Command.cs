using System.Collections;
using UnityEngine;
public enum CommandTypes { None, Move, Attack, Cast, Wait};

[System.Serializable]
public class Command
{
    public CommandTypes type = CommandTypes.None;
    public bool canBeCancelled = true;
    public Vector2 position;

    public void StartCastIndicator(Character character, float time)
    {
        if (character.stats.castRoutine != null) character.StopCoroutine(character.stats.castRoutine);
        if (time > 0) character.StartCoroutine(CastTime(character.stats, time));
        else character.stats.casting = 1;
    }

    public IEnumerator CastTime(RPGStats stats, float time)
    {
        float timePassed = 0;

        while (timePassed < time)
        {
            stats.casting = 1 - (timePassed / time);
            yield return new WaitForFixedUpdate();
            timePassed += Time.fixedDeltaTime;
        }
        stats.castRoutine = null;
    }
}

[System.Serializable]
public class MoveCommand : Command
{
    public void Execute(Character character)
    {
        StartCastIndicator(character, -1);
        character.transform.position = Vector2.MoveTowards(character.transform.position, position, character.moveSpeed * Time.fixedDeltaTime);

        if ((Vector2)character.transform.position == position)
        {
            character.CommandList.RemoveAt(0);
        }
    }
}

[System.Serializable]
public class AttackCommand : Command
{
    public Character target;

    public IEnumerator Execute(Character attacker)
    {
        StartCastIndicator(attacker, -1);
        do
        {
            yield return new WaitForFixedUpdate();

            position = target.position;

            //move towards target
            var range = target.size + attacker.stats.range + attacker.size;

            if (Vector2.Distance(attacker.transform.position, target.transform.position) > range)
            {
                attacker.transform.position = Vector2.MoveTowards(attacker.transform.position, position, attacker.moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                if (Time.time - attacker.lastAttack > attacker.stats.attackSpeed && attacker.CheckIfHasDirectVision(attacker.position, position))
                {
                    target.TakeDamage(attacker.stats.damage, attacker.stats.criticalStrike);
                    if (target.GetType() == typeof(Enemy)) ((Enemy)(target)).AddAggro((Hero)attacker, 1);

                    attacker.lastAttack = Time.time;
                }
            }

            attacker.CollisionWithOtherCharacters();

            // stop attacking if target is dead
            if (target.IsDead || attacker.IsDead)
            {
                if (attacker.commandCoroutine != null) attacker.CommandList.RemoveAt(0);
                attacker.commandCoroutine = null;
            }
        } while (!attacker.CommandHasChanged(this));

        if (attacker.commandCoroutine != null) attacker.StopCoroutine(attacker.commandCoroutine);
        attacker.commandCoroutine = null;
    }
}

[System.Serializable]
public class SpellCommand : Command
{
    public Spell spell;
    public Hero caster;
    public Character target;
    public int spellIndex;

    public void Execute()
    {
        if (spell.requireTarget)
        {
            if (target == null)
            {
                caster.CommandList.RemoveAt(0);
                return;
            }

            position = target.position;
        }

        if (spell.useRange && (Vector2.Distance(caster.position, position) > spell.range + caster.size + (spell.requireTarget ? target.size : 0))) //move towards target 
        {
            caster.CommandList.Insert(0, MoveTowardsTarget(caster.position, target.position));
            return;
        }
        else if (caster.spellCooldowns[spellIndex] < Time.time - spell.cooldown)
        {
            if (spell.manacost > caster.stats.mana)
            {
                caster.CommandList.RemoveAt(0);
                return;
            }

            if ((spell.targetGround && target == null)
                ||(spell.canTargetDead && target.IsDead) 
                || (spell.canTargetLiving && !target.IsDead)
                || (spell.targetEnemies && target.GetType() == typeof(Enemy))
                || (spell.targetAllies && target.GetType() == typeof(Hero)))
            {
                caster.stats.mana -= spell.manacost;
                caster.spellCooldowns[spellIndex] = Time.time; //apply cooldown

                StartCastIndicator(caster, spell.castTime);
                spell.Execute(this);
            }
        }
        else caster.CommandList.RemoveAt(0);
    }

    Command MoveTowardsTarget(Vector2 casterPos, Vector2 targetPos)
    {
        Command command = new MoveCommand
        {
            type = CommandTypes.Move,
            position = casterPos + (targetPos - casterPos) * .1f
        };

        return command;
    }
}

[System.Serializable]
public class WaitCommand : Command
{
    public float waitTime;

    public void Execute(Character character)
    {
        StartCastIndicator(character, waitTime);
        character.commandCoroutine = character.StartCoroutine(Wait(character, waitTime));
    }

    IEnumerator Wait(Character character, float time)
    {
        yield return new WaitForSeconds(time);
        if(character.CommandList[0].GetType() == typeof(WaitCommand)) character.CommandList.RemoveAt(0);
        character.commandCoroutine = null;
    }
}
