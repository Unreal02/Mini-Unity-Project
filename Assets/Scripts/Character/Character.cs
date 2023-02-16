using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Waiting,
        Moving,
        Attacking
    }
    protected State state;

    public int maxHp;
    public int hp;
    public virtual void GetAttacked(int delta)
    {
        hp -= delta - defense;
        HpDecreaseText hpDecreaseText = ObjectPool.Instance<HpDecreaseText>().GetObject().GetComponent<HpDecreaseText>();
        hpDecreaseText.Init(transform.position, delta - defense);
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public int damage;
    public int defense;

    public HpBar hpBar;

    public Weapon weapon;
    protected List<Character> attackOpponent;

    protected Map map;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected const float moveTime = 0.416f;
    protected Vector3 movePosition;
    protected MapTile currentTile;

    protected int GetRadius()
    {
        if (weapon != null) { return weapon.radius; }
        return 1;
    }

    protected int GetDamage()
    {
        if (weapon != null) { return weapon.damage; }
        return damage;
    }

    public virtual void Init()
    {
        hp = maxHp;
        state = State.Idle;
        movePosition = transform.position;
        map = MapManager.Instance.map;
        MapTile tile = map.GetTile((int)transform.position.x, (int)transform.position.y);
        tile.character = this;
        currentTile = tile;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void SetAnimation()
    {
        switch (state)
        {
            case State.Idle:
            case State.Waiting:
                animator.SetTrigger("Idle");
                break;
            case State.Moving:
                animator.SetTrigger("Move");
                break;
            case State.Attacking:
                animator.SetTrigger("Attack");
                break;
        }
    }

    private void SetSpriteFlip(Vector3 facing)
    {
        Vector3 facingDir = facing - transform.position;
        if (facingDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (facingDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    protected virtual void OnStateChange() { }

    public bool IsIdleOrWaiting() { return state == State.Idle || state == State.Waiting; }

    protected void SetState(State value)
    {
        if (state != value)
        {
            state = value;
            SetAnimation();
            switch (value)
            {
                case State.Attacking:
                    StartCoroutine("Attack");
                    break;
            }
            OnStateChange();
        }
    }

    protected void TrySetMovePoint(int dx, int dy)
    {
        if (!map.GetTile((int)movePosition.x + dx, (int)movePosition.y + dy).IsMovable()) { return; }
        movePosition += new Vector3(dx, dy, 0);
        SetSpriteFlip(movePosition);
        SetTileInfo();
        SetState(State.Moving);
    }

    protected void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, Time.deltaTime / moveTime);
        if (transform.position == movePosition)
        {
            SetState(State.Waiting);
        }
    }

    protected void Wait()
    {
        if (CharacterManager.Instance.IsWaitFinished())
        {
            SetState(State.Idle);
        }
    }

    protected void SetTileInfo()
    {
        if (currentTile != null)
        {
            currentTile.character = null;
        }
        int x = Mathf.RoundToInt(movePosition.x);
        int y = Mathf.RoundToInt(movePosition.y);
        MapTile tile = map.GetTile(x, y);
        tile.character = this;
        currentTile = tile;
    }

    protected bool TryAttack(Character opponent)
    {
        Vector3 pos = transform.position;
        Vector3 opponentPos = opponent.transform.position;
        int distance = Mathf.RoundToInt(Utils.L1Distance(pos, opponentPos));
        if (distance > GetRadius()) return false;
        attackOpponent = new List<Character> { opponent };
        SetSpriteFlip(opponent.transform.position);
        SetState(State.Attacking);
        return true;
    }

    protected bool AttackSurrounding()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        attackOpponent = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                MapTile tile = map.GetTile(x + i, y + j);
                if (tile == null || tile.character == null) continue;
                if (tile.character is Enemy)
                {
                    attackOpponent.Add(tile.character);
                }
            }
        }
        if (attackOpponent.Count == 0) return false;
        SetState(State.Attacking);
        return true;
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(moveTime);
        attackOpponent.ForEach(c => c.GetAttacked(GetDamage()));
        SetState(State.Waiting);
    }

    protected virtual void OnDestroy()
    {
        if (hpBar != null)
        {
            hpBar.OnCharacterDestroy();
        }
    }
}
