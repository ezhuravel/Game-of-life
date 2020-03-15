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

            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

            if (hits.Any())
            {
                var liveCells = hits.Where(x => x.collider.gameObject.CompareTag("Live"));

                if (liveCells.Any())
                {
                    foreach(var liveCell in liveCells)
                    {
                        DeleteCell(liveCell.collider.gameObject);
                    }
                }
                else
                {
                    var emptyCell = hits.FirstOrDefault(x => x.collider.gameObject.CompareTag("Empty"));
                    SpawnCell(emptyCell.collider.gameObject);                   
                }
            }
        }

        // pause game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeStep();
            paused = paused ? false : true;
        }

        elapsed += Time.deltaTime;
        if (elapsed >= GameSpeed && !paused)
        {
            elapsed = elapsed % 1f;
           // TakeStep();
        }
    }

    private void TakeStep()
    {
        var deleteList = new List<GameObject>();
        var spawnList = new List<GameObject>();

        var counter = 0;

        //// Determine which cells live and which cell Dies in the next step (generation)
        foreach (Transform t in transform)
        {        
            
            var tileScript = t.GetComponent<Tile>();
            var adjacentLiveCellCount = tileScript.AdjacentLiveCells();

            if (t.CompareTag("Live"))
            {
                if (adjacentLiveCellCount < 2 || adjacentLiveCellCount > 3)
                {
                    deleteList.Add(t.gameObject);
                }
            }
            else
            {
                if (adjacentLiveCellCount == 3)
                {
                    spawnList.Add(t.gameObject);
                }
            }

            counter++;
        }



        foreach (var gameObject in deleteList)
        {
            DeleteCell(gameObject);
        }

        foreach (var gameObject in spawnList)
        {
            SpawnCell(gameObject);
        }

        Debug.Log("Step Complete");
    }

    /// <summary>
    /// Removes the gameobject from the scene and from the Board array
    /// </summary>
    /// <param name="g">The game object to be deleted</param>
    private void DeleteCell(GameObject g)
    {     
        Destroy(g);
    }

    /// <summary>
    /// Created a gameobject where the empty cell g resides
    /// </summary>
    /// <param name="g"></param>
    private void SpawnCell(GameObject g)
    {
        var tileComponent = g.GetComponent<Tile>();

        GameObject liveCell = Instantiate(LiveCell, transform);
        liveCell.name = $"Live Cell";
        liveCell.transform.position = g.transform.position;
    }
}
