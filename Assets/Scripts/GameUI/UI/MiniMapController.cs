using UnityEngine;
using UnityEngine.UI;
using SmallScaleInc.TopDownPixelCharactersPack1;
//using System.ComponentModel;

public class MiniMapController : MonoBehaviour
{
    [Header("Map Image")]
    public Image mapImage;
    [SerializeField] private int textureWidth = 256;
    [SerializeField] private int textureHeight = 256;
    [SerializeField] private float renderInterval = 0.1f;

    [Header("Markers")]
    [SerializeField] private float markerSize = 10f;
    [SerializeField] private Color itemColor = Color.green;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private Color doorColor = Color.yellow;

    private Camera playerCamera;
    private RenderTexture renderTexture;
    private Texture2D mapTexture;
    private Sprite mapSprite;
    private Sprite markerSprite;
    private float nextRenderTime;

    private ItemControl[] itemControllers = new ItemControl[0];
    private EnemyControl[] enemyControllers = new EnemyControl[0];
    private LockedInteractable[] doorTriggers = new LockedInteractable[0];
    private Image[] itemMarkers = new Image[0];
    private Image[] enemyMarkers = new Image[0];
    private Image[] doorMarkers = new Image[0];

    [SerializeField] public GameObject windowPanel;
    [SerializeField] public GameObject windowPanelKey;
    [SerializeField] public GameObject HospitalWindowPanel;
    [SerializeField] public GameObject tutorialNote;
    [SerializeField] public GameObject IsaacNote;

    private void Start()
    {
        InitCamera();
        InitMapTexture();
        RefreshTargets();
    }

    private void LateUpdate()
    {
        if (mapImage == null)
        {
            return;
        }

        if (playerCamera == null)
        {
            InitCamera();
        }

        if (playerCamera == null)
        {
            return;
        }

        if (Time.unscaledTime >= nextRenderTime)
        {
            RenderCameraToImage();
            RefreshTargets();
            nextRenderTime = Time.unscaledTime + renderInterval;
        }

        UpdateMarkers();
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (mapTexture != null)
        {
            Destroy(mapTexture);
        }

        if (mapSprite != null)
        {
            Destroy(mapSprite);
        }

        if (markerSprite != null)
        {
            Destroy(markerSprite);
        }
    }

    private void InitCamera()
    {
        PlayerController playerController = PlayerController.Instance;

        if (playerController != null)
        {
            playerCamera = playerController.PlayerCamera;
        }

        if (playerCamera == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                playerCamera = mainCamera;
            }
        }
    }

    private void InitMapTexture()
    {
        if (mapImage == null)
        {
            return;
        }

        renderTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32);
        mapTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        mapSprite = Sprite.Create(
            mapTexture,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0.5f)
        );

        mapImage.sprite = mapSprite;
        mapImage.preserveAspect = true;
        markerSprite = CreateMarkerSprite();
    }

    private void RenderCameraToImage()
    {
        if (renderTexture == null || mapTexture == null)
        {
            InitMapTexture();
        }

        RenderTexture previousTargetTexture = playerCamera.targetTexture;
        RenderTexture previousActiveTexture = RenderTexture.active;

        bool shouldRestoreWindowPanel = windowPanel != null && windowPanel.activeSelf;
        bool shouldRestoreWindowPanelKey = windowPanelKey != null && windowPanelKey.activeSelf;
        bool shouldRestoreHospitalWindowPanel = HospitalWindowPanel != null && HospitalWindowPanel.activeSelf;
        bool shouldRestoreTutorialWindowPanel = tutorialNote != null && tutorialNote.activeSelf;
        bool shouldRestoreIsaacWindowPanel = IsaacNote != null && IsaacNote.activeSelf;

        if (shouldRestoreWindowPanel)
        {
            windowPanel.SetActive(false);
        }

        if (shouldRestoreWindowPanelKey)
        {
            windowPanelKey.SetActive(false);
        }

        if (shouldRestoreHospitalWindowPanel)
        {
            HospitalWindowPanel.SetActive(false);
        }

        if (shouldRestoreTutorialWindowPanel)
        {
            tutorialNote.SetActive(false);
        }

        if (shouldRestoreIsaacWindowPanel)
        {
            IsaacNote.SetActive(false);
        }

        try
        {
            playerCamera.targetTexture = renderTexture;
            playerCamera.Render();

            RenderTexture.active = renderTexture;
            mapTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
            mapTexture.Apply();
        }

        finally
        {
            if (shouldRestoreWindowPanel)
            {
                windowPanel.SetActive(true);
            }

            if (shouldRestoreWindowPanelKey)
            {
                windowPanelKey.SetActive(true);
            }

            if (shouldRestoreHospitalWindowPanel)
            {
                HospitalWindowPanel.SetActive(true);
            }

            if (shouldRestoreTutorialWindowPanel)
            {
                tutorialNote.SetActive(true);
            }

            if (shouldRestoreIsaacWindowPanel)
            {
                IsaacNote.SetActive(true);
            }

            playerCamera.targetTexture = previousTargetTexture;
            RenderTexture.active = previousActiveTexture;
        }
    }

    private void RefreshTargets()
    {
        ItemControl[] allItems = FindObjectsOfType<ItemControl>();
        var filteredItems = new System.Collections.Generic.List<ItemControl>();

        foreach (var item in allItems)
        {
            var type = item.GetInteractionType();

            if(type == ItemInteractionType.Custom)
            {
                continue;
            }
            filteredItems.Add(item);
        }

        itemControllers = filteredItems.ToArray();
        enemyControllers = FindObjectsOfType<EnemyControl>();
        doorTriggers = FindObjectsOfType<LockedInteractable>();

        EnsureMarkers(ref itemMarkers, itemControllers.Length, "Item Marker", itemColor);
        EnsureMarkers(ref enemyMarkers, enemyControllers.Length, "Enemy Marker", enemyColor);
        EnsureMarkers(ref doorMarkers, doorTriggers.Length, "Door Marker", doorColor);
    }

    private void EnsureMarkers(ref Image[] markers, int count, string markerName, Color markerColor)
    {
        if (markers.Length == count)
        {
            return;
        }

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] != null)
            {
                Destroy(markers[i].gameObject);
            }
        }

        markers = new Image[count];

        for (int i = 0; i < count; i++)
        {
            GameObject markerObject = new GameObject($"{markerName} {i + 1}", typeof(RectTransform), typeof(Image));
            markerObject.transform.SetParent(mapImage.transform, false);

            Image marker = markerObject.GetComponent<Image>();
            marker.sprite = markerSprite;
            marker.color = markerColor;
            marker.raycastTarget = false;

            RectTransform markerRect = marker.rectTransform;
            markerRect.anchorMin = new Vector2(0.5f, 0.5f);
            markerRect.anchorMax = new Vector2(0.5f, 0.5f);
            markerRect.pivot = new Vector2(0.5f, 0.5f);
            markerRect.sizeDelta = new Vector2(markerSize, markerSize);

            markers[i] = marker;
        }
    }

    private void UpdateMarkers()
    {
        UpdateMarkerGroup(itemControllers, itemMarkers);
        UpdateMarkerGroup(enemyControllers, enemyMarkers);
        UpdateMarkerGroup(doorTriggers, doorMarkers);
    }

    private void UpdateMarkerGroup(Component[] targets, Image[] markers)
    {
        RectTransform mapRect = mapImage.rectTransform;
        Rect rect = mapRect.rect;

        for (int i = 0; i < markers.Length; i++)
        {
            Image marker = markers[i];
            Component target = i < targets.Length ? targets[i] : null;

            if (marker == null)
            {
                continue;
            }

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                marker.gameObject.SetActive(false);
                continue;
            }

            Vector3 viewportPoint = playerCamera.WorldToViewportPoint(target.transform.position);
            bool isInsideCamera = viewportPoint.z > 0f
                && viewportPoint.x >= 0f && viewportPoint.x <= 1f
                && viewportPoint.y >= 0f && viewportPoint.y <= 1f;

            marker.gameObject.SetActive(isInsideCamera);

            if (!isInsideCamera)
            {
                continue;
            }

            RectTransform markerRect = marker.rectTransform;
            markerRect.anchoredPosition = new Vector2(
                (viewportPoint.x - 0.5f) * rect.width,
                (viewportPoint.y - 0.5f) * rect.height
            );
            markerRect.sizeDelta = new Vector2(markerSize, markerSize);
        }
    }

    private Sprite CreateMarkerSprite()
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
}
