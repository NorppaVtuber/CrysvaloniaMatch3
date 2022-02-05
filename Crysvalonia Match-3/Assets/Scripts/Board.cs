using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int height;
    public int width;

    public GameObject tilePrefab;

    private BackgroundTile[,] tiles;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new BackgroundTile[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i,j);
                Instantiate(tilePrefab, tempPos, Quaternion.identity);
            }
        }
    }
}
