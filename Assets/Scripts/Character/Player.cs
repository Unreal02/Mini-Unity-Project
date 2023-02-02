using UnityEngine;

public class Player : Character
{
    public override void Init()
    {
        base.Init();
        Debug.Log("player init");
        MainCamera.Instance.player = this;
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
    }

    protected override void OnStateChange()
    {
        switch (state)
        {
            case State.Idle:
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
    }
}
