using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int height;
    public int width;

    public GameObject tilePrefab;
    public GameObject[] objects;

    private BackgroundTile[,] tiles;
    public GameObject[,] allObjects;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new BackgroundTile[width, height];
        allObjects = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";int objectToUse = Random.Range(0, objects.Length);
                Vector3 tilePos = new Vector3(tempPos.x, tempPos.y, -0.03f);
                GameObject _object = Instantiate(objects[objectToUse], tilePos, Quaternion.identity);
                _object.transform.parent = this.transform;
                allObjects[i, j] = _object;
            }
        }
    }
}
