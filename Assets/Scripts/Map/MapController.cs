using UnityEngine;

namespace AppleGrapple
{
    public sealed class MapController : MonoBehaviour
    {
    private const string FenceSortingLayer = "Fences";

        [SerializeField] private MapConfig _mapConfig;
        [SerializeField] private float _boundaryColliderThickness = 0.1f;

        private Transform _mapTilesParent;

        public MapConfig MapConfig => _mapConfig;

        public void Generate()
        {
            Clean();

            if (_mapConfig == null)
            {
                Debug.LogError("MapConfig is not assigned on MapController.", this);
                return;
            }

            var mapTilesParentObject = new GameObject("MapTilesParent");
            _mapTilesParent = mapTilesParentObject.transform;
            _mapTilesParent.SetParent(transform, false);

            for (var x = 0; x < _mapConfig.GridX; x++)
            {
                for (var y = 0; y < _mapConfig.GridY; y++)
                {
                    var xPosition = (x - _mapConfig.GridX * 0.5f + 0.5f) * _mapConfig.tileWorldSize;
                    var yPosition = (y - _mapConfig.GridY * 0.5f + 0.5f) * _mapConfig.tileWorldSize;
                    CreateTile($"Tile_{x}_{y}", new Vector3(xPosition, yPosition, 0f), SelectTileSprite());
                }
            }

            GenerateBoundary();
        }

        public void Clean()
        {
            if (_mapTilesParent == null)
                return;

            Destroy(_mapTilesParent.gameObject);
            _mapTilesParent = null;
        }

        private void OnDestroy()
        {
            Clean();
        }

        private void GenerateBoundary()
        {
            var offset = _mapConfig.boundarySettings.offset;
            GenerateBoundaryColliders(offset);
            GenerateBoundaryFence(offset);
        }

        private void GenerateBoundaryColliders(Vector2 offset)
        {
            var halfWidth = (_mapConfig.GridX * 0.5f + 0.5f) * _mapConfig.tileWorldSize - offset.x;
            var halfHeight = (_mapConfig.GridY * 0.5f + 0.5f) * _mapConfig.tileWorldSize - offset.y;
            var thickness = Mathf.Max(0.01f, _boundaryColliderThickness);

            CreateBoundaryWall("Boundary_Bottom", new Vector2(0f, -halfHeight), new Vector2(halfWidth * 2f + thickness, thickness));
            CreateBoundaryWall("Boundary_Top", new Vector2(0f, halfHeight), new Vector2(halfWidth * 2f + thickness, thickness));
            CreateBoundaryWall("Boundary_Left", new Vector2(-halfWidth, 0f), new Vector2(thickness, halfHeight * 2f + thickness));
            CreateBoundaryWall("Boundary_Right", new Vector2(halfWidth, 0f), new Vector2(thickness, halfHeight * 2f + thickness));
        }

        private void CreateBoundaryWall(string wallName, Vector2 localPosition, Vector2 size)
        {
            var wall = new GameObject(wallName);
            wall.transform.SetParent(_mapTilesParent, false);
            wall.transform.localPosition = localPosition;
            wall.AddComponent<BoxCollider2D>().size = size;
        }

        private void GenerateBoundaryFence(Vector2 offset)
        {
            var spriteA = _mapConfig.boundarySettings.spriteA;
            var spriteB = _mapConfig.boundarySettings.spriteB;
            var spriteC = _mapConfig.boundarySettings.spriteC;
            if (spriteA == null && spriteB == null && spriteC == null)
                return;

            var leftX = (-_mapConfig.GridX * 0.5f - 0.5f) * _mapConfig.tileWorldSize + offset.x;
            var rightX = (_mapConfig.GridX * 0.5f + 0.5f) * _mapConfig.tileWorldSize - offset.x;
            var bottomY = (-_mapConfig.GridY * 0.5f - 0.5f) * _mapConfig.tileWorldSize + offset.y;
            var topY = (_mapConfig.GridY * 0.5f + 0.5f) * _mapConfig.tileWorldSize - offset.y;

            CreateTile("Fence_BottomLeft", new Vector3(leftX, bottomY, 0f), spriteA, 15, null, FenceSortingLayer);
            CreateTile("Fence_BottomRight", new Vector3(rightX, bottomY, 0f), spriteA, 15, null, FenceSortingLayer);
            CreateTile("Fence_TopLeft", new Vector3(leftX, topY, 0f), spriteA, 15, null, FenceSortingLayer);
            CreateTile("Fence_TopRight", new Vector3(rightX, topY, 0f), spriteA, 15, null, FenceSortingLayer);

            var verticalCenterY = (bottomY + topY) * 0.5f;
            var horizontalCenterX = (leftX + rightX) * 0.5f;
            var verticalHeight = Mathf.Max(0f, topY - bottomY);
            var horizontalWidth = Mathf.Max(0f, rightX - leftX);

            CreateTile("Fence_Left", new Vector3(leftX, verticalCenterY, 0f), spriteB, 5,
                new Vector2(_mapConfig.boundarySettings.spriteBSize.x, verticalHeight), FenceSortingLayer);
            CreateTile("Fence_Right", new Vector3(rightX, verticalCenterY, 0f), spriteB, 5,
                new Vector2(_mapConfig.boundarySettings.spriteBSize.x, verticalHeight), FenceSortingLayer);
            CreateTile("Fence_Bottom", new Vector3(horizontalCenterX, bottomY, 0f), spriteC, 5,
                new Vector2(horizontalWidth, _mapConfig.boundarySettings.spriteCSize.y), FenceSortingLayer);
            CreateTile("Fence_Top", new Vector3(horizontalCenterX, topY, 0f), spriteC, 5,
                new Vector2(horizontalWidth, _mapConfig.boundarySettings.spriteCSize.y), FenceSortingLayer);
        }

        private void CreateTile(string tileName, Vector3 localPosition, Sprite sprite, int sortingOrder = -3,
            Vector2? slicedSize = null, string sortingLayerName = null)
        {
            var tileObject = new GameObject(tileName);
            tileObject.transform.SetParent(_mapTilesParent, false);
            tileObject.transform.localPosition = localPosition;
            var spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            if (!string.IsNullOrEmpty(sortingLayerName))
                spriteRenderer.sortingLayerName = sortingLayerName;
            spriteRenderer.sortingOrder = sortingOrder;
            if (slicedSize.HasValue)
            {
                spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                spriteRenderer.size = slicedSize.Value;
            }
        }

        private Sprite SelectTileSprite()
        {
            var randomValue = Random.value;
            var chanceTotal = 0f;

            if (_mapConfig.mapTileVariants != null)
            {
                foreach (var variant in _mapConfig.mapTileVariants)
                {
                    chanceTotal += variant.Chance;
                    if (randomValue <= chanceTotal && variant.tileSprite != null)
                        return variant.tileSprite;
                }
            }

            return _mapConfig.defaultTileSprite;
        }
    }
}