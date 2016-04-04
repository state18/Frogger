using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private GameObject roadTilePrefab;
    [SerializeField]
    private GameObject waterTilePrefab;
    [SerializeField]
    private GameObject divisionTilePrefab;

    // Contains every tile type except division tiles. Used to randomly select the terrain of the next area
    private GameObject[] tileTypes;
    // The weights are relative to each other. The higher the weight, the more frequent the type of tile will occur.
    [SerializeField]
    private float roadTileWeight;
    [SerializeField]
    private float waterTileWeight;

    [SerializeField]
    private int subdivisions;
    [SerializeField]
    private Dictionary<int, GameObject> mapStructure;
    public GameObject[,] Tiles { get; private set; }

    void Start() {
        
        mapStructure = new Dictionary<int, GameObject>();
        tileTypes = new GameObject[] { roadTilePrefab, waterTilePrefab };
        CalculateMapStructure();
        GenerateMap();
    }

    // Update is called once per frame
    void Update() {

    }

    void CalculateMapStructure() {

        // Catch invalid values and set the map parameters to a default map.
        if (mapSize.y < 5 || subdivisions < 1 || subdivisions >= mapSize.y / 2f || waterTileWeight < 0 || roadTileWeight < 0 || waterTileWeight + roadTileWeight <= 0) {
            // There will be 3 intermediate tiles, 1 road, and 1 water.
            mapSize.y = 5;

            mapStructure[1] = divisionTilePrefab;
            mapStructure[2] = roadTilePrefab;
            mapStructure[3] = divisionTilePrefab;
            mapStructure[4] = waterTilePrefab;
            mapStructure[5] = divisionTilePrefab;

            Debug.Log("Invalid parameters, default map created.");
            return;
        }

        // Normalize weights
        roadTileWeight = roadTileWeight / (roadTileWeight + waterTileWeight);
        waterTileWeight = 1 - roadTileWeight;


        // The final subdivision will be subdivisionLength + the remainder below.
        var averageSubdivisionLength = Mathf.CeilToInt((float)mapSize.y / subdivisions);
        var amountToExtendMap = averageSubdivisionLength * subdivisions - mapSize.y + 1;
        mapSize.y += amountToExtendMap;
        // Evenly distribute the subdivisions, with the final subdivision capped by the end of the map.
        for (int i = 0; i < subdivisions; i++) {
            mapStructure[i * averageSubdivisionLength + 1] = divisionTilePrefab;

        }
        mapStructure[mapSize.y] = divisionTilePrefab;

        //foreach (var key in mapStructure.Keys) {
        //    Debug.Log(key);
        //}

        //TODO Divisions have been marked evenly, now fill in the subdivisions with water/road, taking weights into account.

        // The weights aren't that useful for smaller levels, so just alternate terrain types cyclically here.

        GameObject currentTileType = null;
        bool hasDeterminedTileType = false;
        for (int i = 1; i <= mapSize.y; i++) {
            // The map structure will already contain a key for each subdivision tile. This tells us to generate another subdivision of terrain.
            if (mapStructure.ContainsKey(i)) {
                hasDeterminedTileType = false;
                continue;
            }
            // New subdivision encountered. Decide what type of tile will occupy it.
            if (!hasDeterminedTileType) {
                // Small maps will randomly select a tile type different from the previous subdivision. Weights are ignored here.
                if (mapSize.y < 1000) {
                    if (currentTileType != null) {
                        int randomIndex = 0;
                        do {
                            randomIndex = Random.Range(0, tileTypes.Length);
                        } while (tileTypes[randomIndex] == currentTileType);

                        currentTileType = tileTypes[randomIndex];
                        hasDeterminedTileType = true;

                    // Initialize to road type.
                    } else {
                        currentTileType = roadTilePrefab;
                        hasDeterminedTileType = true;
                    }
                } else {
                    // TODO Handle the case of big maps using weights.
                }

                
            }

            mapStructure[i] = currentTileType;
            //Debug.Log(i + " is type: " + currentTileType.name);
        }
    }

    /// <summary>
    /// The actual instantiation of the map tiles
    /// </summary>
    void GenerateMap() {
        for (int y = 1; y <= mapSize.y; y++) {
            for (int x = 1; x <= mapSize.x; x++) {
                var obj = (GameObject)Instantiate(mapStructure[y], new Vector3(x, y), Quaternion.identity);
                // Each tile will be the child of this game object for scene hierarchical organization.
                obj.transform.parent = gameObject.transform;
                
            }
        }
    }

    /// <summary>
    /// The key properties of the map are listed in a string
    /// </summary>
    /// <returns>A neatly formatted string with map properties</returns>
    public override string ToString() {
        return " ";
    }
}
