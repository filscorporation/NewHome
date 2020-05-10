using System.Collections.Generic;
using Assets.Source.Objects;
using UnityEngine;

namespace Assets.Source
{
    /// <summary>
    /// Controlls map tiles and background
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        private static MapManager instance;
        public static MapManager Instance => instance ?? (instance = FindObjectOfType<MapManager>());
        public ObjectManager Targets = new ObjectManager();
        public ObjectManager Enemies = new ObjectManager();
        public ObjectManager Interactables = new ObjectManager();
        public ObjectManager Walls = new ObjectManager();

        [SerializeField] [Range(10, 1000)] private int mapSize = 100;
        [SerializeField] [Range(1, 100)] private int clearRange = 10;

        [SerializeField] private GameObject previewObject;

        [Header("Parallax")]
        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private GameObject backgroundLayer1Prefab;
        [SerializeField] private GameObject backgroundLayer2Prefab;
        [SerializeField] private List<GameObject> clouds;
        private readonly LinkedList<GameObject> groundTiles = new LinkedList<GameObject>();
        private float groundTileWidth;
        private float groundLeftBorder = 0;
        private float groundRightBorder = 0;
        private readonly LinkedList<GameObject> backgroundLayer1Tiles = new LinkedList<GameObject>();
        private float backgroundLayer1TileWidth;
        private float backgroundLayer1LeftBorder = 0;
        private float backgroundLayer1RightBorder = 0;
        [SerializeField] private float layer1ParallaxFactor = 0.2F;
        private readonly LinkedList<GameObject> backgroundLayer2Tiles = new LinkedList<GameObject>();
        private float backgroundLayer2TileWidth;
        private float backgroundLayer2LeftBorder = 0;
        private float backgroundLayer2RightBorder = 0;
        [SerializeField] private float layer2ParallaxFactor = 0.1F;
        private readonly LinkedList<GameObject> cloudsObjects = new LinkedList<GameObject>();
        private float cloudsLeftBorder = 0;
        private float cloudsRightBorder = 0;
        [SerializeField] private float cloudsParallaxFactor = 0.03F;
        [SerializeField] [Range(0.1F, 5F)] private float cloudsDistance = 1F;
        [SerializeField] [Range(0, 1F)] private float cloudsSpeed = 0.05F;
        private float cloudsMinY = -0.5F;
        private float cloudsMaxY = 1.5F;

        private float cameraWidth;

        [SerializeField] public Transform parallaxPivot;
        private float pivotLastTileRebuldingX;
        private const float rebuildingStep = 1F;
        private float pivotLastX;

        [Header("Generation")]
        [SerializeField] private List<GameObject> trees;
        [SerializeField] [Range(0, 1F)] private float treesDensity;
        [SerializeField] private List<GameObject> smallDetails;
        [SerializeField] [Range(0, 1F)] private float smallDetailsDensity;
        [SerializeField] private List<GameObject> bigDetails;
        [SerializeField] [Range(0, 1F)] private float bigDetailsDensity;

        private Random random;

        private void Start()
        {
            if (previewObject != null) previewObject.SetActive(false);

            Initialize();
            DrawTiles();
            GenerateTrees();
            GenerateBigDetails();
            GenerateSmallDetails();
        }

        private void FixedUpdate()
        {
            Targets.Sort();
            Enemies.Sort();
            Interactables.Sort();
            Walls.Sort();
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

            MoveClouds();
        }

        /// <summary>
        /// Returns map size in units
        /// </summary>
        /// <returns></returns>
        public int GetMapSize() => mapSize;

        private void Initialize()
        {
            cameraWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
            Sprite sprite = groundPrefab.GetComponent<SpriteRenderer>().sprite;
            groundTileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            sprite = backgroundLayer1Prefab.GetComponent<SpriteRenderer>().sprite;
            backgroundLayer1TileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            sprite = backgroundLayer2Prefab.GetComponent<SpriteRenderer>().sprite;
            backgroundLayer2TileWidth = sprite.rect.width / sprite.pixelsPerUnit * 0.999F;
            pivotLastX = parallaxPivot.transform.position.x;
            pivotLastTileRebuldingX = parallaxPivot.transform.position.x;
        }

        private void GenerateTrees()
        {
            float pnx = Random.Range(0, 10F);
            float pny = Random.Range(0, 10F);
            float minStep = 0.1F;

            float x = - mapSize / 2F;
            while (x < mapSize / 2F)
            {
                if (Mathf.Abs(x) < clearRange)
                    x = clearRange;

                if (Mathf.PerlinNoise(pnx + 30F * x / mapSize, pny) < treesDensity)
                {
                    AddTree(x);
                }

                float dx = Random.Range(0, 1F);
                x += minStep + dx;
            }
        }

        private void AddTree(float x)
        {
            int treeIndex = Random.Range(0, trees.Count);
            Instantiate(trees[treeIndex], new Vector3(x, trees[treeIndex].transform.position.y), Quaternion.identity, transform);
        }

        private void GenerateBigDetails()
        {
            float minStep = 1F;

            float x = -mapSize / 2F;
            while (x < mapSize / 2F)
            {
                if (Mathf.Abs(x) < clearRange)
                    x = clearRange;

                if (Random.Range(0, 1F) < bigDetailsDensity)
                {
                    AddBigDetail(x);
                }

                float dx = Random.Range(0, 1F);
                x += minStep + dx;
            }
        }

        private void AddBigDetail(float x)
        {
            int detailIndex = Random.Range(0, bigDetails.Count);
            Instantiate(bigDetails[detailIndex], new Vector3(x, bigDetails[detailIndex].transform.position.y), Quaternion.identity, transform);
        }

        private void GenerateSmallDetails()
        {
            float minStep = 0.2F;

            float x = -mapSize / 2F;
            while (x < mapSize / 2F)
            {
                if (Random.Range(0, 1F) < smallDetailsDensity)
                {
                    AddSmallDetail(x);
                }

                float dx = Random.Range(0, 1F);
                x += minStep + dx;
            }
        }

        private void AddSmallDetail(float x)
        {
            int detailIndex = Random.Range(0, smallDetails.Count);
            Instantiate(smallDetails[detailIndex], new Vector3(x, smallDetails[detailIndex].transform.position.y), Quaternion.identity, transform);
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
                    new Vector3(groundLeftBorder - groundTileWidth / 2, groundPrefab.transform.position.y),
                    Quaternion.identity,
                    transform);
                groundTiles.AddFirst(tile);
                groundLeftBorder -= groundTileWidth;
            }
            while (groundRightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    groundPrefab,
                    new Vector3(groundRightBorder + groundTileWidth / 2, groundPrefab.transform.position.y),
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
                    new Vector3(backgroundLayer1LeftBorder - backgroundLayer1TileWidth / 2, backgroundLayer1Prefab.transform.position.y),
                    Quaternion.identity,
                    transform);
                backgroundLayer1Tiles.AddFirst(tile);
                backgroundLayer1LeftBorder -= backgroundLayer1TileWidth;
            }
            while (backgroundLayer1RightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer1Prefab,
                    new Vector3(backgroundLayer1RightBorder + backgroundLayer1TileWidth / 2, backgroundLayer1Prefab.transform.position.y),
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
                    new Vector3(backgroundLayer2LeftBorder - backgroundLayer2TileWidth / 2, backgroundLayer2Prefab.transform.position.y),
                    Quaternion.identity,
                    transform);
                backgroundLayer2Tiles.AddFirst(tile);
                backgroundLayer2LeftBorder -= backgroundLayer2TileWidth;
            }
            while (backgroundLayer2RightBorder < rightX)
            {
                GameObject tile = Instantiate(
                    backgroundLayer2Prefab,
                    new Vector3(backgroundLayer2RightBorder + backgroundLayer2TileWidth / 2, backgroundLayer2Prefab.transform.position.y),
                    Quaternion.identity,
                    transform);
                backgroundLayer2Tiles.AddLast(tile);
                backgroundLayer2RightBorder += backgroundLayer2TileWidth;
            }

            // Removing clouds
            while ((tileToDelete = cloudsObjects.Last?.Value)?.transform.position.x > rightX)
            {
                cloudsObjects.RemoveLast();
                Destroy(tileToDelete);
                cloudsRightBorder -= cloudsDistance;
            }
            while ((tileToDelete = cloudsObjects.First?.Value)?.transform.position.x < leftX)
            {
                cloudsObjects.RemoveFirst();
                Destroy(tileToDelete);
                cloudsLeftBorder += cloudsDistance;
            }
            // Clouds
            while (cloudsLeftBorder >= leftX)
            {
                GameObject tile = AddCloud(cloudsLeftBorder - cloudsDistance / 2);
                cloudsObjects.AddFirst(tile);
                cloudsLeftBorder -= cloudsDistance;
            }
            while (cloudsRightBorder < rightX)
            {
                GameObject tile = AddCloud(cloudsRightBorder + cloudsDistance / 2);
                cloudsObjects.AddLast(tile);
                cloudsRightBorder += cloudsDistance;
            }
        }

        private GameObject AddCloud(float x)
        {
            GameObject prefab = clouds[Random.Range(0, clouds.Count)];
            prefab.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(0, 3);
            return Instantiate(prefab, new Vector3(x, Random.Range(cloudsMinY, cloudsMaxY)), Quaternion.identity, transform);
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
                tile.transform.position += new Vector3(dx * layer1ParallaxFactor, 0);
            }
            backgroundLayer1LeftBorder += dx * layer1ParallaxFactor;
            backgroundLayer1RightBorder += dx * layer1ParallaxFactor;

            foreach (GameObject tile in backgroundLayer2Tiles)
            {
                tile.transform.position += new Vector3(dx * layer2ParallaxFactor, 0);
            }
            backgroundLayer2LeftBorder += dx * layer2ParallaxFactor;
            backgroundLayer2RightBorder += dx * layer2ParallaxFactor;

            foreach (GameObject tile in cloudsObjects)
            {
                tile.transform.position += new Vector3(dx * cloudsParallaxFactor, 0);
            }
            cloudsLeftBorder += dx * cloudsParallaxFactor;
            cloudsRightBorder += dx * cloudsParallaxFactor;
        }

        private void MoveClouds()
        {
            foreach (GameObject tile in cloudsObjects)
            {
                tile.transform.position += new Vector3(cloudsSpeed * Time.deltaTime, 0);
            }
            cloudsLeftBorder += cloudsSpeed * Time.deltaTime;
            cloudsRightBorder += cloudsSpeed * Time.deltaTime;
        }
    }
}
