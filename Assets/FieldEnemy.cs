using TMPro;
using UnityEngine;

public class FieldEnemy : MonoBehaviour
{
    public SphereCollider searchRadius;
    Rigidbody rb;
    float speed = 0.05f;
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 moveDir = Vector3.Lerp(rb.position, target.position, Time.fixedDeltaTime * speed);
            rb.MovePosition(moveDir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player (1)")
        {
            Debug.Log(gameObject.name + " detected " + other.gameObject.name);
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player (1)")
        {
            Debug.Log(gameObject.name + " lost " + other.gameObject.name);
            target = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player (1)")
        {
            Debug.Log("Enemy touched player -- Entering Battle");
            GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);
        }
    }
}
