using UnityEngine;

public class HpBar : MonoBehaviour
{
    private Character character;
    private Transform bar;
    private float lifeTime;

    public void Init(Character c)
    {
        character = c;
        bar = transform.GetChild(1).GetChild(0);
        lifeTime = 3f;
    }

    private void Update()
    {
        if (character == null) return;
        if (character is Enemy)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0f)
            {
                ReturnToPool();
                return;
            }
        }
        float ratio = (float)character.hp / (float)character.maxHp;
        bar.localScale = new Vector3(ratio, 1, 1);
        bar.localPosition = new Vector3(ratio / 2.0f - 0.5f, 0, -0.3f);
        transform.localPosition = character.transform.position;
    }

    public void OnCharacterDestroy()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        character.hpBar = null;
        character = null;
        ObjectPool.Instance<HpBar>().ReturnObject(gameObject);
    }
}
