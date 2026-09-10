using UnityEngine;
namespace AppleGrapple
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private float boundaryColliderThickness = 0.1f;
        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            if (mapConfig == null)
            {
                Debug.LogWarning("Map Config could not found !");
                return;
            }
            for (var i = 0; i < mapConfig.GridX ; i++)
            {
                for (var k = 0; k < mapConfig.GridY ; k++)
                {
                    var xPos = (i - mapConfig.GridX * 0.5f + 0.5f) * mapConfig.tileWorldSize;
                    var yPos = (k - mapConfig.GridY * 0.5f + 0.5f) * mapConfig.tileWorldSize;
                    CreateTile($"Tile_{i}_{k}", transform, new Vector3(xPos, yPos, 0f), SelectTileSprite());
                }
            } 
            GenerateBoundary();
        }

        private void GenerateBoundary()
        {
            var offset = mapConfig.boundarySettings.offset;
            GenerateBoundaryColliders(offset);
            GenerateBoundaryFence(offset);
        }

        private void GenerateBoundaryColliders(Vector2 offset)
        {
            var halfWidth = (mapConfig.GridX * 0.5f + 0.5f) * mapConfig.tileWorldSize - offset.x;
            var halfHeight = (mapConfig.GridY * 0.5f + 0.5f) * mapConfig.tileWorldSize - offset.y;
            var thickness = Mathf.Max(0.01f, boundaryColliderThickness);

            CreateBoundaryWall("Boundary_Bottom", new Vector2(0f, -halfHeight), new Vector2(halfWidth * 2f + thickness, thickness));
            CreateBoundaryWall("Boundary_Top", new Vector2(0f, halfHeight), new Vector2(halfWidth * 2f + thickness, thickness));
            CreateBoundaryWall("Boundary_Left", new Vector2(-halfWidth, 0f), new Vector2(thickness, halfHeight * 2f + thickness));
            CreateBoundaryWall("Boundary_Right", new Vector2(halfWidth, 0f), new Vector2(thickness, halfHeight * 2f + thickness));
        }

        private void CreateBoundaryWall(string name, Vector2 localPosition, Vector2 size)
        {
            var wall = new GameObject(name);
            wall.transform.SetParent(transform, false);
            wall.transform.localPosition = localPosition;
            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        // Rings one tile outside the grid so the fence sits just past the edge, then pulls it inward by the config offset.
        private void GenerateBoundaryFence(Vector2 offset)
        {
            var spriteA = mapConfig.boundarySettings.spriteA;
            var spriteB = mapConfig.boundarySettings.spriteB;
            var spriteC = mapConfig.boundarySettings.spriteC;
            if (spriteA == null && spriteB == null && spriteC == null) return;

            var leftX = (-mapConfig.GridX * 0.5f - 0.5f) * mapConfig.tileWorldSize + offset.x;
            var rightX = (mapConfig.GridX * 0.5f + 0.5f) * mapConfig.tileWorldSize - offset.x;
            var bottomY = (-mapConfig.GridY * 0.5f - 0.5f) * mapConfig.tileWorldSize + offset.y;
            var topY = (mapConfig.GridY * 0.5f + 0.5f) * mapConfig.tileWorldSize - offset.y;

            CreateTile("Fence_BottomLeft", transform, new Vector3(leftX, bottomY, 0f), spriteA, 5);
            CreateTile("Fence_BottomRight", transform, new Vector3(rightX, bottomY, 0f), spriteA, 5);
            CreateTile("Fence_TopLeft", transform, new Vector3(leftX, topY, 0f), spriteA, 5);
            CreateTile("Fence_TopRight", transform, new Vector3(rightX, topY, 0f), spriteA, 5);

            var verticalCenterY = (bottomY + topY) * 0.5f;
            var horizontalCenterX = (leftX + rightX) * 0.5f;
            var verticalHeight = Mathf.Max(0f, topY - bottomY);
            var horizontalWidth = Mathf.Max(0f, rightX - leftX);

            CreateTile("Fence_Left", transform, new Vector3(leftX, verticalCenterY, 0f), spriteB, 5,
                new Vector2(mapConfig.boundarySettings.spriteBSize.x, verticalHeight));
            CreateTile("Fence_Right", transform, new Vector3(rightX, verticalCenterY, 0f), spriteB, 5,
                new Vector2(mapConfig.boundarySettings.spriteBSize.x, verticalHeight));
            CreateTile("Fence_Bottom", transform, new Vector3(horizontalCenterX, bottomY, 0f), spriteC, 5,
                new Vector2(horizontalWidth, mapConfig.boundarySettings.spriteCSize.y));
            CreateTile("Fence_Top", transform, new Vector3(horizontalCenterX, topY, 0f), spriteC, 5,
                new Vector2(horizontalWidth, mapConfig.boundarySettings.spriteCSize.y));
        }

        private void CreateTile(string name, Transform parent, Vector3 localPosition, Sprite sprite, int sortingOrder = 0, Vector2? slicedSize = null)
        {
            var tileObject = new GameObject(name);
            tileObject.transform.SetParent(parent, false);
            tileObject.transform.localPosition = localPosition;
            var spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
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

            if (mapConfig.mapTileVariants != null)
            {
                foreach (var variant in mapConfig.mapTileVariants)
                {
                    chanceTotal += variant.Chance;
                    if (randomValue <= chanceTotal && variant.tileSprite != null)
                    {
                        return variant.tileSprite;
                    }
                }
            }

            return mapConfig.defaultTileSprite;
        }
    }
}

