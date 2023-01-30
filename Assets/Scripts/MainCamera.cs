using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player player;

    const float scrollScale = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = player.transform.position;
        position.z = transform.position.z;
        transform.position = position;

        if (Input.mouseScrollDelta.y != 0)
        {

            Camera.main.orthographicSize = Mathf.Exp(Mathf.Clamp(Mathf.Log(Camera.main.orthographicSize) - Input.mouseScrollDelta.y * scrollScale, 1, 50));
        }
    }
}
