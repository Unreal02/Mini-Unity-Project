using UnityEngine;

public class Player : Character
{
    public MapTile GetCurrentTile() { return currentTile; }
    public Vector3 GetMovePosition() { return movePosition; }

    public override void Init()
    {
        base.Init();
        MainCamera.Instance.player = this;
        hpBar = ObjectPool.Instance<HpBar>().GetObject().GetComponent<HpBar>();
        hpBar.Init(this);
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
                if (GetCurrentTile().type == MapTile.TileType.Goal)
                {
                    GameManager.Instance.stageClearEvent.Invoke();
                }
                break;
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

            // Ctrl + click: 범위 공격 (상하 + 좌우 + 대각선 1칸)
            if (Input.GetKey(KeyCode.LeftControl))
            {
                AttackSurrounding();
            }

            // 일반 공격 (단일 대상)
            else
            {
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

    public void MoveTo(int x, int y)
    {
        movePosition = new Vector3(x, y);
        transform.position = movePosition;
        map = MapManager.Instance.map;
        SetTileInfo();
        SetState(State.Idle);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.playerDeathEvent.Invoke();
    }
}
