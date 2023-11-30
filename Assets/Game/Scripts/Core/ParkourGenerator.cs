using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkourGenerator : MonoBehaviour
{
    [SerializeField] private Transform environmentRoot;
    [Header("Parkour Generation")]
    [SerializeField] private Tile groundTilePrefab;
    [SerializeField] private TileData groundTileData;
    [SerializeField] private Transform rootPoint;
    [SerializeField] private int sizeX, sizeY;
    private List<List<GameObject>> parkours = new List<List<GameObject>>();
    private List<GameObject> parkourParents = new List<GameObject>();

    [Header("Stack Tile")]
    [SerializeField] private Tile stackTilePrefab;
    [SerializeField] private TileData stackTileData;
    private List<GameObject> stackTiles = new List<GameObject>();
    private GameObject stackTilesParent;

    [Header("Stack Tile Trigger for Bridge")]
    [SerializeField] private GameObject stackTileTriggerPrefab;
    [SerializeField] private Tile bridgeVisual;
    [SerializeField] private TileData bridgeVisualData;
    [SerializeField] private int count = 5;
    [SerializeField] private bool horizontal = true;

    public void GenerateParkour()
    {
        Vector3 spawnPoint = rootPoint.position;
        Vector3 alteredRootPoint = spawnPoint + Vector3.left * (((float)sizeX - 1) / 2);
        spawnPoint = alteredRootPoint;

        var parkour = new List<GameObject>();
        var parkourParent = new GameObject();
        parkourParent.name = "Parkour";
        parkourParent.isStatic = true;
        parkourParents.Add(parkourParent);

        for (int i = 0; i < sizeY; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                var generatedGroundTile = Instantiate(groundTilePrefab);
                generatedGroundTile.transform.position = spawnPoint;
                generatedGroundTile.transform.SetParent(parkourParent.transform);
                spawnPoint.x += groundTilePrefab.transform.localScale.x;
                // set color
                generatedGroundTile.data = groundTileData;
                generatedGroundTile.Initialize();

                parkour.Add(generatedGroundTile.gameObject);
            }
            spawnPoint = alteredRootPoint;
            spawnPoint.z += groundTilePrefab.transform.localScale.z * (i + 1);
        }
        parkourParent.transform.SetParent(environmentRoot);
        parkours.Add(parkour);
    }

    public void ClearAllParkours()
    {
        foreach (var parkour in parkours)
        {
            foreach (var tile in parkour)
            {
                DestroyImmediate(tile);
            }
            parkour.Clear();
        }
        parkours.Clear();

        stackTiles.ForEach(x => DestroyImmediate(x));
        stackTiles.Clear();
        DestroyImmediate(stackTilesParent);

        parkourParents.ForEach(x => DestroyImmediate(x));
        parkourParents.Clear();
    }

    public void AddStackTilesToAllParkours()
    {
        if (stackTiles.Any())
        {
            stackTiles.ForEach(x => DestroyImmediate(x));
            stackTiles.Clear();
            DestroyImmediate(stackTilesParent);
        }

        stackTilesParent = new GameObject();
        stackTilesParent.name = "Stack Tiles";
        stackTilesParent.isStatic = true;

        foreach (var parkour in parkours)
        {
            foreach (var tile in parkour)
            {
                if (!tile.activeInHierarchy)
                {
                    var generatedStackTile = Instantiate(stackTilePrefab);
                    generatedStackTile.transform.position = tile.transform.position;
                    generatedStackTile.transform.SetParent(stackTilesParent.transform);
                    // set color
                    generatedStackTile.data = stackTileData;
                    generatedStackTile.Initialize();

                    stackTiles.Add(generatedStackTile.gameObject);
                }
            }
        }

        stackTilesParent.transform.SetParent(environmentRoot);
    }

    public void GenerateBridgeTileTriggers()
    {
        var bridgeTriggersParent = new GameObject();
        bridgeTriggersParent.name = "TileTriggerParent";

        for (int i = 0; i < count; i++)
        {
            var generatedTrigger = Instantiate(stackTileTriggerPrefab) as GameObject;
            var spawnPoint = rootPoint.position;

            if (horizontal)
                spawnPoint.x += (stackTilePrefab.transform.localScale.x + 0.1f) * i;
            else
                spawnPoint.z += (stackTilePrefab.transform.localScale.z + 0.1f) * i;

            generatedTrigger.transform.position = spawnPoint;
            generatedTrigger.transform.SetParent(bridgeTriggersParent.transform);
        }

        var generatedBridgeVisual = Instantiate(bridgeVisual) as Tile;
        var newBridgeVisualPos = rootPoint.position;

        if (horizontal)
            newBridgeVisualPos.x += (count - 1) / 2f * 1.1f - 0.1f;
        else
            newBridgeVisualPos.z += (count - 1) / 2f * 1.1f - 0.1f;

        newBridgeVisualPos.y -= 0.2f;
        generatedBridgeVisual.transform.position = newBridgeVisualPos;

        var newBridgeVisualScale = generatedBridgeVisual.transform.localScale;

        if (horizontal)
        {
            newBridgeVisualScale.x = count + 1;
            newBridgeVisualScale.z = 0.3f;
        }
        else
        {
            newBridgeVisualScale.z = count + 1;
            newBridgeVisualScale.x = 0.3f;
        }

        generatedBridgeVisual.transform.localScale = newBridgeVisualScale;
        generatedBridgeVisual.data = bridgeVisualData;
        generatedBridgeVisual.Initialize();

        generatedBridgeVisual.transform.SetParent(bridgeTriggersParent.transform);
    }
}
