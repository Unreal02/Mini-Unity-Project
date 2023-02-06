using UnityEngine;

public class Enemy : Character
{
    private void Start()
    {
        GameManager.Instance.playerActionEvent.AddListener(EnemyAI);
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
            SetMovePoint(dx, dy);
        }
    }
}
