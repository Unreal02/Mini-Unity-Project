using TMPro;
using UnityEngine;

public class HpDecreaseText : MonoBehaviour
{
    private float lifeTime;
    private TextMeshPro text;
    private const float scrollSpeed = 0.3f;

    public void Init(Vector3 pos, int val)
    {
        lifeTime = 1f;
        transform.position = pos + new Vector3(0, 0.5f, 0);
        text = GetComponent<TextMeshPro>();
        text.text = val.ToString();
        text.color = Color.red;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        transform.position += Vector3.up * Time.deltaTime * scrollSpeed;
        Color color = text.color;
        color.a -= Time.deltaTime;
        text.color = color;
        if (lifeTime < 0f)
        {
            ObjectPool.Instance<HpDecreaseText>().ReturnObject(gameObject);
            return;
        }
    }
}
