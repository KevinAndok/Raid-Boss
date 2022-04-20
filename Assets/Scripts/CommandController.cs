using UnityEngine;

public class CommandController : MonoBehaviour
{
    public static CommandController instance;

    public LayerMask _characterLayers;

    public KeyCode[] spellHotkeys;

    RaycastHit2D TryGetCharacter() => Physics2D.BoxCast(Mouse.worldPosition, Vector2.one * .1f, 0, Vector2.zero, 0, _characterLayers.value);

    private void Awake()
    {
        if (!instance) instance = this;
    }

    private void Update()
    {
        if (Player.selectedCharacters.Length > 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                var target = TryGetCharacter();

                if (target && target.transform.TryGetComponent(out Enemy enemy)) Attack(enemy);
                else Move();
            }
            else
            {
                for (int i = 0; i < spellHotkeys.Length; i++)
                {
                    if (Input.GetKeyDown(spellHotkeys[i]))
                    {
                        Hero hero = Player.selectedCharacters[Player.controllingCharacter];
                        var target = TryGetCharacter();
                        Cast(hero, i, target ? target.transform.GetComponent<Character>() : null, Mouse.worldPosition);
                    }
                }
            }
        }
    }

    public void Move()
    {
        Command command = new MoveCommand
        {
            type = CommandTypes.Move,
            position = Mouse.worldPosition + new Vector2(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f))
        };

        foreach (Hero hero in Player.selectedCharacters)
        {
            SetCommand(hero, command);
        }
    }

    public void Attack(Character target)
    {
        if (target.IsDead) return;

        Command command = new AttackCommand
        {
            type = CommandTypes.Attack,
            target = target,
            position = target.position
        };

        foreach (Hero hero in Player.selectedCharacters)
        {
            SetCommand(hero, command);
        }
    }

    public void Cast(Hero hero, int spellIndex, Character target, Vector2 mousePosition)
    {
        if (hero.spells.Length <= spellIndex) return;

        var spell = hero.spells[spellIndex];

        if (spell.requireTarget && !target) return;

        Command command = new SpellCommand
        {
            type = CommandTypes.Cast,
            position = mousePosition,
            target = target,
            spell = spell,
            caster = hero,
            canBeCancelled = spell.canBeCanceled,
            spellIndex = spellIndex
        };

        SetCommand(hero, command);
    }

    private static void SetCommand(Hero hero, Command command)
    {
        if (hero.IsDead) return;

        if (Input.GetKey(KeyCode.LeftShift) || (hero.Channelling && !hero.CommandList[0].canBeCancelled)) hero.QueueCommand(command);
        else if (command.GetType() == typeof(SpellCommand) && ((SpellCommand)command).spell.dontCancelAttack && hero.CommandList.Count > 0 && hero.CommandList[0].GetType() == typeof(AttackCommand)) hero.CommandList.Insert(0, command);
        else if (hero.CommandList.Count > 0 && !hero.TryCancelCommand(hero.CommandList[0])) hero.QueueCommand(command);
        else hero.CreateCommand(command);
    }
}
