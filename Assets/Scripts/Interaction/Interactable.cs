using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    public float interactionRadius = 1f;
    public bool isInteractable;
    public string interactKey = "E";
    public GameObject interact;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (interact != null)
            interact.SetActive(false);
    }
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if(distance <= interactionRadius)
        {
            if (!isInteractable)
                isInteractable = true;
            if (interact != null)
                interact.SetActive(true);

            if (Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }
        else
        {
            if(isInteractable)
                isInteractable = false;
            if (interact != null)
                interact.SetActive(false);
        }
    }

    void Interact()
    {
        Debug.Log($"You interacted with {gameObject.name}");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
