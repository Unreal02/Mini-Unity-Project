using UnityEngine;

public class Player : MonoBehaviour
{
    enum State
    {
        Idle,
        Moving
    }
    State state;

    Map map;
    Animator animator;
    SpriteRenderer spriteRenderer;

    const float moveTime = 0.416f;
    Vector3 movePosition;

    public void Init()
    {
        state = State.Idle;
        movePosition = transform.position;
        map = MapManager.Instance.map;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                GetInput();
                break;
            case State.Moving:
                Move();
                break;
        }

        MoveCamera();
    }

    private void SetState(State value)
    {
        if (state != value)
        {
            state = value;
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
    }

    private void GetInput()
    {
        int dx = 0, dy = 0;
        if (Input.GetKey(KeyCode.W)) { dy += 1; }
        if (Input.GetKey(KeyCode.A)) { dx += -1; }
        if (Input.GetKey(KeyCode.S)) { dy += -1; }
        if (Input.GetKey(KeyCode.D)) { dx += 1; }
        if (dx == 0 ^ dy == 0)
        {
            if (map.GetTile((int)transform.position.x + dx, (int)transform.position.y + dy).type == MapTile.TileType.Ground)
            {
                SetMovePoint(dx, dy);
                SetState(State.Moving);
            }
        }
    }

    public void MoveTo(int x, int y)
    {
        transform.position = new Vector3(x, y, 0);
    }

    private void SetMovePoint(int dx, int dy)
    {
        movePosition += new Vector3(dx, dy, 0);
        if (dx < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (dx > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, Time.deltaTime / moveTime);
        if (transform.position == movePosition)
        {
            SetState(State.Idle);
        }
    }

    private void MoveCamera()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.z = Camera.main.transform.position.z;
        Camera.main.transform.position = cameraPosition;
    }
}
