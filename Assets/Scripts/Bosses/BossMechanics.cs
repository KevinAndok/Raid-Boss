using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossType { Andy }; //used for boss setup from menu if theres more bosses

/*
    TODO: difficulty scaling
*/

public class BossMechanics : MonoBehaviour
{
    public static BossMechanics instance;

    public bool fightStarted;

    [SerializeField]
    public Boss bossController;

    private void Awake()
    {
        instance = this;

        bossController.DifficultySetup();
    }

    private void Start()
    {
        bossController.InitiateBattleield();
    }

    private void Update()
    {
        CheckFughtStart();
    }

    private void FixedUpdate()
    {
        bossController.UpdateBehaviour();
    }

    void CheckFughtStart()
    {
        if (!fightStarted)
        {
            bool bossesAgroed = false;

            foreach (Enemy boss in Player.instance.bosses)
            {
                if (boss.aggroPoints.Count > 0)
                {
                    bossesAgroed = true;
                    break;
                }
            }

            if (bossesAgroed)
            {
                bossController.StartFight();
                fightStarted = true;
            }
        }
    }

    //TODO: display gizmos circles for every boss spawn location
}
