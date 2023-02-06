using UnityEngine;

public class Player : Character
{
    public override void Init()
    {
        base.Init();
        MainCamera.Instance.player = this;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                GetInput();
                break;
            case State.Waiting:
                Wait();
                break;
            case State.Moving:
                Move();
                break;
        }
    }

    protected override void OnStateChange()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Waiting:
                break;
            case State.Moving:
                GameManager.Instance.playerActionEvent.Invoke();
                break;
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
            if (map.GetTile((int)transform.position.x + dx, (int)transform.position.y + dy).IsMovable())
            {
                SetMovePoint(dx, dy);
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            GameManager.Instance.playerActionEvent.Invoke();
            SetState(State.Waiting);
        }
    }
}
