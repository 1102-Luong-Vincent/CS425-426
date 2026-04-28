using UnityEngine;

public class bossfix : MonoBehaviour
{
    public Vector3 FixedPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = FixedPosition;
    }
}
