using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class RhythmGameManage : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI ScoreText;

    [Header("Game Objects")]
    public GameObject CheckRange;                
    public GameObject ArrowPrefab;                 
    public Transform ArrowPrefabListTransform;     

    [Header("Settings")]
    public float spawnInterval = 2f;   
    public float arrowSpeed = 50f;     

    private int score = 0;
    private float spawnTimer = 0f;

    public static RhythmGameManage Instance { get; private set; }
    private List<ArrowControl> activeArrows = new List<ArrowControl>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ShowScoreText();
    }


    private void Update()
    {
        HandleSpawn();
        HandleInput();

    }

    void ShowScoreText()
    {
        ScoreText.text = $"Score : {score}";
    }

    void HandleSpawn()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnArrow();
        }
    }

    void SpawnArrow()
    {
        GameObject arrow = Instantiate(ArrowPrefab, ArrowPrefabListTransform);
        arrow.transform.localPosition = new Vector3(900f, 0f, 0f);

        ArrowControl arrowCtrl = arrow.GetComponent<ArrowControl>();
        arrowCtrl.Init(arrowSpeed);

        activeArrows.Add(arrowCtrl);
    }
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            CheckHit(ArrowDirection.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            CheckHit(ArrowDirection.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            CheckHit(ArrowDirection.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            CheckHit(ArrowDirection.Right);
        if (Input.GetKeyDown(KeyCode.Space))
            CheckHit(ArrowDirection.Space);
    }


    void CheckHit(ArrowDirection inputDirection)
    {
        CircleCollider2D checkCollider = CheckRange.GetComponent<CircleCollider2D>();
        if (checkCollider == null) return;

        for (int i = activeArrows.Count - 1; i >= 0; i--)
        {
            ArrowControl arrow = activeArrows[i];
            Collider2D arrowCollider = arrow.GetComponent<Collider2D>();

            if (arrowCollider != null && checkCollider.bounds.Intersects(arrowCollider.bounds))
            {
                if (arrow.GerArrowDirection() == inputDirection)
                {
                    score += 1;
                    arrow.DestroyArrow();
                    break; 
                } else
                {
                    arrow.DestroyArrow();
                    break;
                }
            }
        }

        ShowScoreText();
    }

    public void RemoveArrow(ArrowControl arrow)
    {
        if (activeArrows.Contains(arrow))
        {
            activeArrows.Remove(arrow);
        }
    }
}

