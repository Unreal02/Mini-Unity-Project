using UnityEngine;

public class Character : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Waiting,
        Moving
    }
    protected State state;

    public int hp;
    public int attack;
    public int defense;

    protected Map map;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected const float moveTime = 0.416f;
    protected Vector3 movePosition;
    protected MapTile currentTile;

    public virtual void Init()
    {
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
                animator.SetTrigger("Idle");
                break;
            case State.Moving:
                animator.SetTrigger("Move");
                break;
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
            OnStateChange();
        }
    }

    protected void TrySetMovePoint(int dx, int dy)
    {
        if (!map.GetTile((int)movePosition.x + dx, (int)movePosition.y + dy).IsMovable()) { return; }
        movePosition += new Vector3(dx, dy, 0);
        if (dx < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (dx > 0)
        {
            spriteRenderer.flipX = false;
        }
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

    private void SetTileInfo()
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
}
