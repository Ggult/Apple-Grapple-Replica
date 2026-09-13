using ScratchCardAsset;
using UnityEngine;

namespace AppleGrapple
{
    public class ScratchManager : MonoBehaviour
    {
        [SerializeField] private ScratchCardManager scratchCardManager;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private SpriteRenderer bottomLayerScratchSpriteRenderer;
        private static ScratchManager _instance;

        private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (scratchCardManager == null)
        {
            var scratchCardManagerInScene = FindFirstObjectByType<ScratchCardManager>();
            if (scratchCardManagerInScene != null)
            {
                 scratchCardManager = scratchCardManagerInScene;
            }
            else
            {
                Debug.LogWarning("ScratchCardManager is not assigned in ScratchManager!");
            }
        }

        ApplyScratchCardTransform();
        DisableMouseScratching();
        scratchCardManager.gameObject.SetActive(true);
        bottomLayerScratchSpriteRenderer.gameObject.SetActive(true);
    }

        private void DisableMouseScratching()
    {
        if (scratchCardManager != null && scratchCardManager.Card != null)
        {
            scratchCardManager.InputEnabled = false;
        }
    }

        private void ApplyScratchCardTransform()
    {
        if (mapConfig == null || scratchCardManager == null || scratchCardManager.SpriteRendererCard == null)
            return;

        var spriteTransform = scratchCardManager.SpriteRendererCard.transform;
        var xScale = CalculateScale(mapConfig.GridX * mapConfig.tileWorldSize);
        var yScale = CalculateScale(mapConfig.GridY * mapConfig.tileWorldSize);

        spriteTransform.localScale = new Vector3(xScale, yScale, 1f);
        bottomLayerScratchSpriteRenderer.transform.localScale = new Vector3(xScale, yScale, 1f);
        bottomLayerScratchSpriteRenderer.transform.localPosition = new Vector3(spriteTransform.localPosition.x - .3f, spriteTransform.localPosition.y - .3f, spriteTransform.localPosition.z);
    }

        private static float CalculateScale(float worldSize)
    {
        // Calibrated from the scene measurements: 40 world units -> 2x, 50 -> 3x.
        return Mathf.Max(0f, (worldSize - 20f) / 10f);
    }

        public static void Scratch(Vector3 worldPosition)
    {
        if (_instance == null || _instance.scratchCardManager == null || _instance.mainCamera == null)
            return;

        var screenPosition = _instance.mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
            return;

        var texturePosition = _instance.scratchCardManager.Card.ScratchData.GetScratchPosition(screenPosition);
        _instance.scratchCardManager.Card.ScratchHole(texturePosition);
        }

    }
}
