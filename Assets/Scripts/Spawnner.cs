using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Tilemaps;

public class Spawnner : MonoBehaviour
{
    [SerializeField] private List<GameObject> tetrisStorer = new();
    [SerializeField] private GameHandler gameHandler;
    void Start() => 
        GenerateTetrisBlock();

    void OnEnable()
    {
        gameHandler.canSpawnNewTetris -= GenerateTetrisBlock;
        gameHandler.canSpawnNewTetris += GenerateTetrisBlock;
    }

    void OnDisable() =>
        gameHandler.canSpawnNewTetris -= GenerateTetrisBlock;
    public void GenerateTetrisBlock()
    {
        int tetrisBlocks = tetrisStorer.Count;
        int currentTetris = Random.Range(0, tetrisBlocks);
        Instantiate(tetrisStorer[currentTetris], transform.position, Quaternion.identity);
    }
}
