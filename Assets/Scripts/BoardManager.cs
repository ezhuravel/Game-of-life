using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int Columns = 0;
    public int Rows = 0;
    public float TileSize = 1;

    public GameObject Cell;
    public float[,] Grid;
    public Camera Camera;

    // Start is called before the first frame update
    void Start()
    {
        Grid = new float[Columns, Rows];
        Camera.orthographicSize = Rows / 2 ;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject cell = Instantiate(Cell, transform);
                cell.name = $"Cell: i:{i} j{j}";
                cell.transform.position = new Vector2(j * TileSize, i * -TileSize);
            }
        }
      
        float gridW = Columns * TileSize;
        float gridH = Rows * TileSize;

        transform.position = new Vector2(-gridW / 2 + TileSize / 2, gridH / 2 - TileSize / 2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
