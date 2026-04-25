using System.Collections;
using UnityEngine;

public class BattleItemSpriteEffect : MonoBehaviour
{
    public Sprite sprite;
    Vector3 startPos;
    Vector3 endPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        endPos = new Vector3(startPos.x, startPos.y + 0.5f, startPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlayEffect());
    }

    public IEnumerator PlayEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the effect in seconds
        while(elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        elapsedTime = 0f;
        duration = 1.0f; // Duration of the fade-out effect in seconds
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        Destroy(gameObject);
    }
}
