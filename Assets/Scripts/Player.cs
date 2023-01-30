using UnityEngine;

public class Player : MonoBehaviour
{
    public void MoveTo(int x, int y)
    {
        transform.position = new Vector3(x, y, 0);
    }
}
