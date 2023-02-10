using UnityEngine;

public class MainCamera : MonoBehaviour
{
    const float scrollScale = 0.3f;
    const float scrollMin = 1f;
    const float scrollMax = 4.9f;
    public Player player;

    static MainCamera instance;
    public static MainCamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MainCamera>();
            }
            return instance;
        }
    }

    private void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        MoveCamera();
        ScrollCamera();
    }

    private void MoveCamera()
    {
        if (player == null) return;
        Vector3 cameraPosition = player.transform.position;
        cameraPosition.z = transform.position.z;
        cameraPosition.y += 0.5f;
        transform.position = cameraPosition;
    }

    private void ScrollCamera()
    {
        float logSize = Mathf.Log(Camera.main.orthographicSize);
        if (Input.mouseScrollDelta.y != 0)
        {
            logSize -= Input.mouseScrollDelta.y * scrollScale;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            logSize += scrollScale;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            logSize -= scrollScale;
        }
        Camera.main.orthographicSize = Mathf.Exp(Mathf.Clamp(logSize, scrollMin, scrollMax));
    }
}
