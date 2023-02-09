using UnityEngine;

public class Player : Character
{
    public MapTile GetCurrentTile() { return currentTile; }
    public Vector3 GetMovePosition() { return movePosition; }

    public override void Init()
    {
        base.Init();
        MainCamera.Instance.player = this;
        hpBar = ObjectPool.Instance.GetHpBar(this);
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
            case State.Moving:
            case State.Attacking:
                GameManager.Instance.playerActionEvent.Invoke();
                break;
        }
    }

    private void GetInput()
    {
        // Move (keyboard)
        int dx = 0, dy = 0;
        if (Input.GetKey(KeyCode.W)) { dy += 1; }
        if (Input.GetKey(KeyCode.A)) { dx += -1; }
        if (Input.GetKey(KeyCode.S)) { dy += -1; }
        if (Input.GetKey(KeyCode.D)) { dx += 1; }
        if (dx == 0 ^ dy == 0)
        {
            TrySetMovePoint(dx, dy);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            GameManager.Instance.playerActionEvent.Invoke();
            SetState(State.Waiting);
        }

        // Attack (mouse)
        if (Input.GetMouseButtonDown(0))
        {
            Grid grid = FindObjectOfType<Grid>();
            Vector3 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(clickPoint.x - grid.transform.position.x - 0.5f);
            int y = Mathf.RoundToInt(clickPoint.y - grid.transform.position.y - 0.5f);

            MapTile tile = map.GetTile(x, y);
            if (tile == null || tile.character == null) return;
            if (tile.character is Enemy)
            {
                TryAttack(tile.character);
            }
        }
    }
}
