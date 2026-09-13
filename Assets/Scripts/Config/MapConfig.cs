using System.Collections.Generic;
using UnityEngine;
using System;
namespace AppleGrapple
{
    [Serializable]
    public struct MapTileVariants
    {
        [Range(0f, 1f)]
        public float Chance;
        public Sprite tileSprite;
    }
    [Serializable]
    public struct BoundarySettings
    {
        public Vector2 offset;
        public Sprite spriteA;
        public Sprite spriteB;
        public Sprite spriteC;
        public Vector2 spriteBSize;
        public Vector2 spriteCSize;
    }
    [CreateAssetMenu(fileName = "MapConfig", menuName = "AppleGrapple/Map Config")]
    public class MapConfig : ScriptableObject
    {
        public float tileWorldSize = 1f;
        public int GridX, GridY;
        public List<MapTileVariants> mapTileVariants;
        public Sprite defaultTileSprite;
        public BoundarySettings boundarySettings;
        
    }
}