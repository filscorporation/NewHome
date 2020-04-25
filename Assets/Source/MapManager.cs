using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source
{
    /// <summary>
    /// Controlls map tiles and background
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private GameObject previewObject;

        [SerializeField] private float groundHeight = 0F;
        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private float backgroundLayer1Height = 0F;
        [SerializeField] private GameObject backgroundLayer1Prefab;
        [SerializeField] private float backgroundLayer2Height = 0F;
        [SerializeField] private GameObject backgroundLayer2Prefab;
        private readonly LinkedList<GameObject> groundTiles = new LinkedList<GameObject>();
        private float groundTileWidth;
        private float groundLeftBorder = 0;
        private float groundRightBorder = 0;
        private readonly LinkedList<GameObject> backgroundLayer1Tiles = new LinkedList<GameObject>();
        private float backgroundLayer1TileWidth;
        private float backgroundLayer1LeftBorder = 0;
        private float backgroundLayer1RightBorder = 0;
        [SerializeField] private float layer1ParallaxFactor = 0.8F;
        private readonly LinkedList<GameObject> backgroundLayer2Tiles = new LinkedList<GameObject>();
        private float backgroundLayer2TileWidth;
        private float backgroundLayer2LeftBorder = 0;
        private float backgroundLayer2RightBorder = 0;
        [SerializeField] private float layer2ParallaxFactor = 0.9F;
        private float cameraWidth;

        [SerializeField] public Transform parallaxPivot;
        private float pivotLastTileRebuldingX;
        private const float rebuildingStep = 1F;
        private float pivotLastX;

        private void Start()
        {
            if (previewObject != null) previewObject.SetActive(false);

            cameraWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
            Sprite sprite = groundPrefab.GetComponent<SpriteRenderer>().sprite;
            groundTileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            sprite = backgroundLayer1Prefab.GetComponent<SpriteRenderer>().sprite;
            backgroundLayer1TileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            sprite = backgroundLayer2Prefab.GetComponent<SpriteRenderer>().sprite;
            backgroundLayer2TileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            pivotLastX = parallaxPivot.transform.position.x;
            pivotLastTileRebuldingX = parallaxPivot.transform.position.x;

            DrawTiles();
        }

        private void Update()
        {
            float dx = parallaxPivot.transform.position.x - pivotLastTileRebuldingX;
            // If pivot didn't really move, then there is no reason to rebuild tiles
            if (Mathf.Abs(dx) > rebuildingStep)
            {
                DrawTiles();
                pivotLastTileRebuldingX = parallaxPivot.transform.position.x;
            }
            Parallax();

            pivotLastX = parallaxPivot.transform.position.x;
        }

        /// <summary>
        /// Draws tiles that only should be visible to the camera
        /// </summary>
        private void DrawTiles()
        {
            float leftX = parallaxPivot.position.x - cameraWidth - rebuildingStep * 2;
            float rightX = parallaxPivot.position.x + cameraWidth + rebuildingStep * 2;
            GameObject tileToDelete = null;

            // Removing ground
            while ((tileToDelete = groundTiles.Last?.Value)?.transform.position.x > rightX)
            {
                groundTiles.RemoveLast();
                Destroy(tileToDelete);
                groundRightBorder -= groundTileWidth;
            }
            while ((tileToDelete = groundTiles.First?.Value)?.transform.position.x < leftX)
            {
                groundTiles.RemoveFirst();
                Destroy(tileToDelete);
                groundLeftBorder += groundTileWidth;
            }
            // Adding new ground
            while (groundLeftBorder >= leftX)
            {
                GameObject tile = Instantiate(
                    groundPrefab,
                    new Vector3(groundLeftBorder - groundTileWidth / 2, groundHeight),
                    Quaternion.identity,
                    transform);
                groundTiles.AddFirst(tile);
                groundLeftBorder -= groundTileWidth;
            }
            while (groundRightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    groundPrefab,
                    new Vector3(groundRightBorder + groundTileWidth / 2, groundHeight),
                    Quaternion.identity,
                    transform);
                groundTiles.AddLast(tile);
                groundRightBorder += groundTileWidth;
            }

            // Removing background layer 1
            while ((tileToDelete = backgroundLayer1Tiles.Last?.Value)?.transform.position.x > rightX)
            {
                backgroundLayer1Tiles.RemoveLast();
                Destroy(tileToDelete);
                backgroundLayer1RightBorder -= backgroundLayer1TileWidth;
            }
            while ((tileToDelete = backgroundLayer1Tiles.First?.Value)?.transform.position.x < leftX)
            {
                backgroundLayer1Tiles.RemoveFirst();
                Destroy(tileToDelete);
                backgroundLayer1LeftBorder += backgroundLayer1TileWidth;
            }
            // Background layer 1
            while (backgroundLayer1LeftBorder >= leftX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer1Prefab,
                    new Vector3(backgroundLayer1LeftBorder - backgroundLayer1TileWidth / 2, backgroundLayer1Height),
                    Quaternion.identity,
                    transform);
                backgroundLayer1Tiles.AddFirst(tile);
                backgroundLayer1LeftBorder -= backgroundLayer1TileWidth;
            }
            while (backgroundLayer1RightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer1Prefab,
                    new Vector3(backgroundLayer1RightBorder + backgroundLayer1TileWidth / 2, backgroundLayer1Height),
                    Quaternion.identity,
                    transform);
                backgroundLayer1Tiles.AddLast(tile);
                backgroundLayer1RightBorder += backgroundLayer1TileWidth;
            }

            // Removing background layer 2
            while ((tileToDelete = backgroundLayer2Tiles.Last?.Value)?.transform.position.x > rightX)
            {
                backgroundLayer2Tiles.RemoveLast();
                Destroy(tileToDelete);
                backgroundLayer2RightBorder -= backgroundLayer2TileWidth;
            }
            while ((tileToDelete = backgroundLayer2Tiles.First?.Value)?.transform.position.x < leftX)
            {
                backgroundLayer2Tiles.RemoveFirst();
                Destroy(tileToDelete);
                backgroundLayer2LeftBorder += backgroundLayer2TileWidth;
            }
            // Background layer 2
            while (backgroundLayer2LeftBorder >= leftX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer2Prefab,
                    new Vector3(backgroundLayer2LeftBorder - backgroundLayer2TileWidth / 2, backgroundLayer2Height),
                    Quaternion.identity,
                    transform);
                backgroundLayer2Tiles.AddFirst(tile);
                backgroundLayer2LeftBorder -= backgroundLayer2TileWidth;
            }
            while (backgroundLayer2RightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer2Prefab,
                    new Vector3(backgroundLayer2RightBorder + backgroundLayer2TileWidth / 2, backgroundLayer2Height),
                    Quaternion.identity,
                    transform);
                backgroundLayer2Tiles.AddLast(tile);
                backgroundLayer2RightBorder += backgroundLayer2TileWidth;
            }
        }

        /// <summary>
        /// Animate parallax effect
        /// </summary>
        private void Parallax()
        {
            // Shifting background sprites to oppotite direction of camera movement
            float dx = parallaxPivot.transform.position.x - pivotLastX;

            foreach (GameObject tile in backgroundLayer1Tiles)
            {
                tile.transform.position -= new Vector3(dx * layer1ParallaxFactor, 0);
            }
            backgroundLayer1LeftBorder -= dx * layer1ParallaxFactor;
            backgroundLayer1RightBorder -= dx * layer1ParallaxFactor;

            foreach (GameObject tile in backgroundLayer2Tiles)
            {
                tile.transform.position -= new Vector3(dx * layer2ParallaxFactor, 0);
            }
            backgroundLayer2LeftBorder -= dx * layer2ParallaxFactor;
            backgroundLayer2RightBorder -= dx * layer2ParallaxFactor;
        }
    }
}
