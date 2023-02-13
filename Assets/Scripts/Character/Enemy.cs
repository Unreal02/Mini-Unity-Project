using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private void Start()
    {
        GameManager.Instance.playerActionEvent.AddListener(EnemyAI);
    }

    public override void GetAttacked(int delta)
    {
        base.GetAttacked(delta);
        if (hpBar == null)
        {
            hpBar = ObjectPool.Instance<HpBar>().GetObject().GetComponent<HpBar>();
        }
        hpBar.Init(this);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Moving:
                Move();
                break;
            case State.Waiting:
                Wait();
                break;
        }
    }

    private void EnemyAI()
    {
        Player player = CharacterManager.Player;
        MapTile playerCurrentTile = player.GetCurrentTile();
        Vector3 playerMovePosition = player.GetMovePosition();
        if (TryAttack(player)) return;
        if (playerCurrentTile.roomId != 0 && playerCurrentTile.roomId == currentTile.roomId
         || Utils.L1Distance(playerMovePosition, transform.position) <= 4)
        {
            MoveTorwardPlayer();
        }
        else
        {
            MoveRandom();
        }
    }

    private void MoveTorwardPlayer()
    {
        Vector3 playerMovePosition = CharacterManager.Player.GetMovePosition();
        int px = Mathf.RoundToInt(playerMovePosition.x);
        int py = Mathf.RoundToInt(playerMovePosition.y);
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int dx = px - x;
        int dy = py - y;
        if (dx == 0)
        {
            if (Mathf.Abs(py - y) > 1) TrySetMovePoint(0, (int)Mathf.Sign(dy));
        }
        else if (dy == 0)
        {
            if (Mathf.Abs(px - x) > 1) TrySetMovePoint((int)Mathf.Sign(dx), 0);
        }
        else
        {
            MapTile xDirTile = map.GetTile(x + (int)Mathf.Sign(dx), y);
            MapTile yDirTile = map.GetTile(x, y + (int)Mathf.Sign(dy));

            if (!xDirTile.IsMovable()) { TrySetMovePoint(0, (int)Mathf.Sign(dy)); }
            else if (!yDirTile.IsMovable()) { TrySetMovePoint((int)Mathf.Sign(dx), 0); }
            else if (!xDirTile.IsMovable() && !yDirTile.IsMovable()) { return; }
            else if (Random.value > 0.5)
            {
                TrySetMovePoint((int)Mathf.Sign(dx), 0);
            }
            else
            {
                TrySetMovePoint(0, (int)Mathf.Sign(dy));
            }
        }
    }

    private void MoveRandom()
    {
        int direction = Random.Range(0, 4);
        int dx = 0, dy = 0;
        switch (direction)
        {
            case 0: dx = 1; break;
            case 1: dx = -1; break;
            case 2: dy = 1; break;
            case 3: dy = -1; break;
        }
        if (map.GetTile((int)transform.position.x + dx, (int)transform.position.y + dy).IsMovable())
        {
            TrySetMovePoint(dx, dy);
        }
    }
}
