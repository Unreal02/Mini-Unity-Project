using UnityEngine;

public class MainCamera : MonoBehaviour
{
    const float scrollScale = 0.3f;

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.orthographicSize = Mathf.Exp(Mathf.Clamp(Mathf.Log(Camera.main.orthographicSize) - Input.mouseScrollDelta.y * scrollScale, 1, 50));
        }
    }
}
