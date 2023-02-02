using UnityEngine;

public class MainCamera : MonoBehaviour
{
    const float scrollScale = 0.3f;
    const float scrollMin = 1f;
    const float scrollMax = 4.9f;
    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        MoveCamera();
        ScrollCamera();
    }

    private void MoveCamera()
    {
        Vector3 cameraPosition = player.transform.position;
        cameraPosition.z = transform.position.z;
        cameraPosition.y += 0.5f;
        transform.position = cameraPosition;
    }

    private void ScrollCamera()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.orthographicSize = Mathf.Exp(Mathf.Clamp(Mathf.Log(Camera.main.orthographicSize) - Input.mouseScrollDelta.y * scrollScale, scrollMin, scrollMax));
        }
    }
}
