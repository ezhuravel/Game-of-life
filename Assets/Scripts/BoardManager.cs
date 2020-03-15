using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public int Columns = 0;
    public int Rows = 0;
    public float TileSize = 1;
    public GameObject EmptyCell;
    public GameObject LiveCell;
    public Camera Camera;
    public float GameSpeed = 1;

    float elapsed = 0f;
    //GameObject[,] Board;
    private bool paused = true;
 
    void Start()
    {
       // Board = new GameObject[Rows, Columns];

        SetUpBoard();    

        Camera.orthographicSize = Columns / 2 ;

        float gridW = Columns * TileSize;
        float gridH = Rows * TileSize;

        transform.position = new Vector2(-gridW / 2 + TileSize / 2, gridH / 2 - TileSize / 2);
    }

    private void SetUpBoard()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject emptyTile = Instantiate(EmptyCell, transform);
                emptyTile.name = $"EmptyTile";
                emptyTile.transform.position = new Vector2(j * TileSize, i * -TileSize);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                var cell = hit.collider.gameObject.GetComponent<Tile>() ;

                if (cell.IsAlive())
                {
                    cell.Die();
                }
                else
                {
                    cell.Born();
                }               
            }
        }

        // pause game
        if (Input.GetKeyDown(KeyCode.Space))
        {          
            paused = paused ? false : true;
        }

        elapsed += Time.deltaTime;
        if (elapsed >= GameSpeed && !paused)
        {
            elapsed = elapsed % 1f;
            TakeStep();
        }
    }

    private void TakeStep()
    {
        var killList = new List<Tile>();
        var bornList = new List<Tile>();

        //// Determine which cells live and which cell Dies in the next step (generation)
        foreach (Transform t in transform)
        {                    
            var tileScript = t.GetComponent<Tile>();
            var adjacentLiveCellCount = tileScript.AdjacentLiveCells();            

            if (tileScript.IsAlive())
            {
                if (adjacentLiveCellCount < 2 || adjacentLiveCellCount > 3)
                {
                    killList.Add(tileScript);
                }
            }
            else
            {
                if (adjacentLiveCellCount == 3)
                {
                    bornList.Add(tileScript);
                }
            }
        }

        foreach (var tile in killList)
        {
            tile.Die();
        }

        foreach (var tile in bornList)
        {
            tile.Born();
        }    
    }
}
