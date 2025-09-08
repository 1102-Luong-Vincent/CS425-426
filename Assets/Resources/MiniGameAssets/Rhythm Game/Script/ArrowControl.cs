using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ArrowDirection
{
    Left,Right,Up,Down,Space
}

public class ArrowControl : MonoBehaviour
{
    public Image ArrowImage; 
    private float speed;
    ArrowDirection currentDirection;
    public Sprite Space;

    public void Init(float moveSpeed)
    {
        speed = moveSpeed;
        RandomSetArrowDirection();
    }

    private void Update()
    {
        transform.localPosition += Vector3.left * speed * Time.deltaTime;
        if (transform.localPosition.x < -1000f)
        {
            DestroyArrow();
        }
    }

    void RandomSetArrowDirection()
    {
        currentDirection = (ArrowDirection)Random.Range(0, System.Enum.GetValues(typeof(ArrowDirection)).Length);
        ArrowImage.sprite = GetArrowSprite(GerArrowDirection());
    }

    public ArrowDirection GerArrowDirection()
    {
        return currentDirection;
    }

    public void DestroyArrow()
    {
        if (RhythmGameManage.Instance != null)
        {
            RhythmGameManage.Instance.RemoveArrow(this);
        }

        Destroy(gameObject);
    }


    public Sprite GetArrowSprite(ArrowDirection direction) // maybe need to write GetSprite Script t
    {
        switch (direction)
        {
            case ArrowDirection.Left: return Resources.Load<Sprite>("Sprite/MiniGame/RhythmGame/LeftArrow");
            case ArrowDirection.Up: return Resources.Load<Sprite>("Sprite/MiniGame/RhythmGame/UpArrow");
            case ArrowDirection.Down: return Resources.Load<Sprite>("Sprite/MiniGame/RhythmGame/DownArrow");
            case ArrowDirection.Right: return Resources.Load<Sprite>("Sprite/MiniGame/RhythmGame/RightArrow");
            case ArrowDirection.Space: return Space;
            default: return null;

        }
    } 

}
