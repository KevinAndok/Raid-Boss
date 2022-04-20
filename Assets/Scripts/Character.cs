using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class Character : MonoBehaviour
{
    [Header("Character")]
    public float size;
    public float moveSpeed;
    public RPGStats stats;
    public bool invulnerable;

    [Header("Other")]
    public GameObject selectionIndicatorObject;
    [SerializeField] List<Command> _commands;
    [SerializeField] Polyline _pathLine;

    [HideInInspector] public Vector2 position;
    [HideInInspector] public bool selected;
    [HideInInspector] public float lastAttack = 0;
    [HideInInspector] public Coroutine commandCoroutine;
    [HideInInspector] public Disc selectionIndicatorShape;

    Transform _selectioonIndicatorTransform;
    float _selectionRotation = 0;

    public List<Command> CommandList => _commands;
    public bool Channelling => commandCoroutine != null;
    public bool IsDead => stats.health <= 0f;

    [HideInInspector] public StatusBars statusBars; //Use to create cast time animation for spells

    public void Awake()
    {
        _selectioonIndicatorTransform = selectionIndicatorObject.transform;
        if (_pathLine) _pathLine = Instantiate(_pathLine);
        selectionIndicatorObject.TryGetComponent(out selectionIndicatorShape);
    }

    private void Start()
    {
        statusBars = Instantiate(Player.instance.statusBarPrefab).GetComponent<StatusBars>();
        statusBars.Initiate(transform.localScale.x, transform.localScale.y);
    }

    private void FixedUpdate()
    {
        if (!IsDead)
        {
            ExecuteCommands();
            StatsRegeneration();
        }

        SelectionIndicatorAnimation();
    }

    public void Update()
    {
        position = transform.position;

        if (_pathLine) UpdateLinePoints();

        HidingStatusBars(IsDead);
    }

    private void LateUpdate()
    {
        statusBars.transform.position = transform.position;
        statusBars.UpdateHealthBar(stats.health / stats.maxHealth);
        statusBars.UpdateManaBar(stats.mana / stats.maxMana);
        statusBars.UpdateCastBar(CommandList.Count > 0 ? stats.casting : 0);
    }

    void HidingStatusBars(bool dead)
    {
        if (statusBars != null) 
        if (statusBars.gameObject.activeInHierarchy == dead) statusBars.gameObject.SetActive(!dead);
    }

    public void CreateCommand(Command command)
    {
        if (CommandList.Count > 0) _commands.Clear();
        CommandList.Add(command);
    }

    public void QueueCommand(Command command)
    {
        CommandList.Add(command);
    }

    void UpdateLinePoints()
    {
        var points = new Vector3[_commands.Count + 1];

        foreach (Command c in _commands)
        {
            if (c.type == CommandTypes.Attack) c.position = ((AttackCommand)c).target.position;
            else if (c.type == CommandTypes.Cast && ((SpellCommand)c).target != null && ((SpellCommand)c).spell.requireTarget) c.position = ((SpellCommand)c).target.position;
            //ADD ALL OTHER COMMANDS WITH MOVING TARGETS HERE
        }

        for (int i = 0; i < points.Length; i++)
        {
            if (i == 0) points[i] = transform.position;
            else points[i] = _commands[i - 1].position;
        }

        _pathLine.SetPoints(points);
        _pathLine.Color = selected ? Color.white : Color.clear;
    }

    void StatsRegeneration()
    {
        stats.health = Mathf.Clamp(stats.health + stats.healthRegeneration * Time.fixedDeltaTime, 0, stats.maxHealth);
        stats.mana = Mathf.Clamp(stats.mana + stats.manaRegeneration * Time.fixedDeltaTime, 0, stats.maxMana);
    }

    public void TakeDamage(float amount, float critChance)
    {
        if (IsDead) return;
        amount = invulnerable ? 0 : (amount * (Random.value < critChance ? 2 : 1) * (1 - stats.resistance));
        stats.health = Mathf.Clamp(stats.health - amount, -1, stats.maxHealth);
        FloatingText.CreateFloatingText(position, Mathf.RoundToInt(amount).ToString(), FloatingText.TextColor.red);
    }
    public void RestoreHealth(float amount, float critChance)
    {
        amount = IsDead ? 0 : (amount * (Random.value < critChance ? 2 : 1));
        stats.health = Mathf.Clamp(stats.health + amount, -1, stats.maxHealth);
        FloatingText.CreateFloatingText(position, amount.ToString(), FloatingText.TextColor.green);
    }
    public void RestoreMana(float amount, float critChance)
    {
        amount = IsDead ? 0 : (amount * (Random.value < critChance ? 2 : 1));
        stats.mana = Mathf.Clamp(stats.mana + amount, -1, stats.maxMana);
        FloatingText.CreateFloatingText(position, amount.ToString(), FloatingText.TextColor.blue);
    }

    public bool CollisionWithOtherCharacters()
    {
        var hits = Physics2D.CircleCastAll(transform.position, size, Vector2.zero, 0, Player.instance.characterCollisionLayers);

        if (hits.Length > 1)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform != transform)
                {
                    Vector2 vector2 = ((Vector2)transform.position - (Vector2)hit.transform.position);
                    transform.position = Vector3.Lerp(transform.position, vector2.normalized * moveSpeed * Time.fixedDeltaTime * 3 + (Vector2)transform.position, .25f);
                    return true; //colliding and moving to an empty position
                }
            }
        }
        return false; //not colliding
    }

    public bool CheckIfHasDirectVision(Vector2 posOne, Vector2 posTwo)
    {
        return !Physics2D.Raycast(posOne, posTwo - posOne, Vector2.Distance(posOne, posTwo), LayerMask.GetMask("Obstacles").GetHashCode());
    }

    void SelectionIndicatorAnimation()
    {
        if (selectionIndicatorObject.activeInHierarchy != selected) selectionIndicatorObject.SetActive(selected);

        if (selectionIndicatorObject.activeInHierarchy)
        {
            _selectioonIndicatorTransform.rotation = Quaternion.Euler(0, 0, _selectionRotation);
            _selectionRotation -= 100 * Time.fixedDeltaTime;
        }
    }

    #region Commands
    void ExecuteCommands()
    {
        if (_commands.Count > 0)
        {
            if (_commands[0].type == CommandTypes.None) _commands.RemoveAt(0); //in case command was wrongly created 
            else if (_commands[0].type == CommandTypes.Move) ((MoveCommand)_commands[0]).Execute(this);
            else if (_commands[0].type == CommandTypes.Attack)
            {
                if (!Channelling) commandCoroutine = StartCoroutine(((AttackCommand)_commands[0]).Execute(this));
            }
            else if (_commands[0].type == CommandTypes.Cast)
            {
                if (!Channelling) ((SpellCommand)_commands[0]).Execute();
            }
            else if (_commands[0].type == CommandTypes.Wait)
            {
                if (!Channelling) ((WaitCommand)_commands[0]).Execute(this);
            }
            //add other commands here
        }
        else
        {
            CollisionWithOtherCharacters();
        }
    }

    public bool TryCancelCommand(Command c)
    {
        if (c.GetType() == _commands[0].GetType()) return true; //dont cancel cast by mistakenly double clicking
        if (!c.canBeCancelled) return false;
        if (Channelling)
        {
            StopCoroutine(commandCoroutine);
            commandCoroutine = null;
        }
        if (_commands.Count > 0) _commands.RemoveAt(0);
        return true;
    }

    public bool CommandHasChanged(Command command) //returns true if command has changed
    {
        if (_commands.Count > 0) return command == _commands[0] ? false : true;
        return true;
    }
    #endregion

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size + stats.range);
    }
    #endif
}
