using UnityEngine;


public interface IMonsterState
{
    void Enter(FieldMonster monster);
    void Update(FieldMonster monster);
    void Exit(FieldMonster monster);
}

public class PatrolState : IMonsterState
{
    private float patrolTimer;
    private float thinkTimer;
    private bool isThinking;

    public void Enter(FieldMonster monster)
    {
        isThinking = false;
        patrolTimer = monster.patrolDuration;
        ChooseDirection(monster);
        monster.anim.SetBool("Walk", true);
    }

    public void Update(FieldMonster monster)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (monster.CanSeePlayer(player))
        {
            monster.ChangeState(new TraceState());
            return;
        }

        if (isThinking)
        {
            thinkTimer -= Time.deltaTime;
            if (thinkTimer <= 0)
            {
                isThinking = false;
                patrolTimer = monster.patrolDuration;
                ChooseDirection(monster);
                monster.anim.SetBool("Walk", true);
            }
        }
        else
        {
            patrolTimer -= Time.deltaTime;
            monster.Move(monster.patrolDirection);

            if (patrolTimer <= 0)
            {
                isThinking = true;
                thinkTimer = monster.thinkDuration;
                monster.anim.SetBool("Walk", false);
            }
        }
    }

    public void Exit(FieldMonster monster)
    {
        monster.anim.SetBool("Walk", false);
    }

    private void ChooseDirection(FieldMonster monster)
    {
        Vector3 dir = (Random.Range(0, 2) == 0) ? Vector3.left : Vector3.right;

        float futureDist = Vector3.Distance(monster.spawnPoint, monster.transform.position + dir * monster.moveSpeed * monster.patrolDuration);
        if (futureDist > monster.maxDistanceFromSpawn)
            dir = -dir;

        monster.patrolDirection = dir;
    }
}

public class TraceState : IMonsterState
{
    public void Enter(FieldMonster monster)
    {
        monster.anim.SetBool("Walk", true);
    }

    public void Update(FieldMonster monster)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (!monster.CanSeePlayer(player))
        {
            monster.ChangeState(new PatrolState());
            return;
        }

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);

        if (dist < monster.attackRange)
        {
            monster.ChangeState(new AttackState(player));
            return;
        }

        Vector3 dir = (player.transform.position - monster.transform.position).normalized;
        monster.Move(dir);
    }

    public void Exit(FieldMonster monster)
    {
        monster.anim.SetBool("Walk", false);
    }
}

public class AttackState : IMonsterState
{
    private Player target;

    public AttackState(Player player)
    {
        target = player;
    }

    public void Enter(FieldMonster monster)
    {
        // 공격 준비 상태
    }

    public void Update(FieldMonster monster)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (!monster.CanSeePlayer(player))
        {
            monster.ChangeState(new PatrolState());
            return;
        }

        float dist = Vector3.Distance(monster.transform.position, player.transform.position);

        if (dist > monster.attackRange)
        {
            monster.ChangeState(new TraceState());
            return;
        }

        if (Time.time >= monster.lastAttackTime + monster.attackCooldown)
        {
            Vector3 dir = (player.transform.position - monster.transform.position).normalized;
            monster.Flip(dir);

            monster.anim.SetTrigger("Attack");
            monster.Attack(player);

            monster.lastAttackTime = Time.time;
        }
    }

    public void Exit(FieldMonster monster) { }
}