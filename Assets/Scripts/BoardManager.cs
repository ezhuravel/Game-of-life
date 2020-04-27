using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public int Columns = 0;
    public int Rows = 0;
    public float TileSize = 1;
    public GameObject CellGameObject;
    public Camera Camera;
    public float GameSpeed = 1;
    public bool WithCaves;
    public int seed = 2;
    public int fillPercent = 20;
    public bool paused = false;
    public Text StepText;
    public Text AliveCellText;
    public Text DeadCellText;
    public Text Ratio;
    public int pauseOn; 

    float elapsed = 0f;
   
    private int step = 0;
    private float ratio = 0;
    List<float> ratiosList;
    private int[,] map;
   
    void Start()
    {

        if(WithCaves)
            SetUpBoardWithCaves();
        else
            SetUpBoard();

        ratiosList = new List<float>();

        Camera.orthographicSize = Columns / 2 ;

        float gridW = Columns * TileSize;
        float gridH = Rows * TileSize;

        transform.position = new Vector2(-gridW / 2 + TileSize / 2, gridH / 2 - TileSize / 2);
    }

    private void SetUpBoard()
    {
        System.Random r = new System.Random(seed);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject emptyTile = Instantiate(CellGameObject, transform);
                emptyTile.name = $"Tile Row: {i} Column {j}";
                emptyTile.transform.position = new Vector2(j * TileSize, i * -TileSize);

                var tileScript = emptyTile.GetComponent<TileBase>();

                var val = r.Next(2);
                if (val == 1)
                    tileScript.tileState = TileState.Alive;
                else
                    tileScript.tileState = TileState.Dead;
            }
        }
    }

    private void SetUpBoardWithCaves()
    {
        System.Random r = new System.Random(seed);
        map = new int[Rows, Columns];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject emptyTile = Instantiate(CellGameObject, transform);
                emptyTile.name = $"Tile Row: {i} Column {j}";
                emptyTile.transform.position = new Vector2(j * TileSize, i * -TileSize);

                var tileScript = emptyTile.GetComponent<TileBase>();

                if(map[i,j] == 1)
                {
                    tileScript.tileState = TileState.Barrier;
                }
                else
                {
                    tileScript.tileState = TileState.Dead;

                    var val = r.Next(2);
                    if (val == 1)
                        tileScript.tileState = TileState.Alive;
                    else
                        tileScript.tileState = TileState.Dead;
                }
            }
        }
    }

    private void SmoothMap()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(i, j);

                if (neighbourWallTiles > 4)
                    map[i, j] = 1;
                else if (neighbourWallTiles < 4)
                    map[i, j] = 0;

            }
        }
    }

    private int GetSurroundingWallCount(int i, int j)
    {
        int wallCount = 0;
        for (int neighbourX = i - 1; neighbourX <= i + 1; neighbourX++)
        {
            for (int neighbourY = j - 1; neighbourY <= j + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < Rows && neighbourY >= 0 && neighbourY < Columns)
                {
                    if (neighbourX != i || neighbourY != j)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private void RandomFillMap()
    {
        System.Random r = new System.Random(2);        

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (i == 0 || i == Rows - 1 || j == 0 || j == Columns - 1)
                {
                    map[i, j] = 1;
                }
                else
                {
                    map[i, j] = (r.Next(0, 100) < fillPercent) ? 1 : 0;
                }
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
                var cell = hit.collider.gameObject.GetComponent<TileBase>() ;

                if (cell.tileState == TileState.Alive)
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

        if(step == pauseOn)
        {
            paused = true;
            Ratio.text = $"Average Ratio: {ratiosList.Average()}";
        }

        elapsed += Time.deltaTime;
        if (elapsed >= GameSpeed && !paused)
        {
            step++;
            StepText.text = $"Step: {step}"; 

            elapsed = elapsed % 1f;
            
            MoveCells();
            TakeStep();

            var totalCells = Rows * Columns;
            var liveCells = GetComponentsInChildren<TileBase>().Count(x => x.tileState == TileState.Alive);
            var deadCells = GetComponentsInChildren<TileBase>().Count(x => x.tileState == TileState.Dead);

            ratio = (float)liveCells / deadCells;

            AliveCellText.text = $"Live Cells: {liveCells}";
            DeadCellText.text = $"Dead cells: {deadCells}";
            Ratio.text = $"A/D: {ratio}";
            ratiosList.Add(ratio);
        }
    }

    private void MoveCells()
    {
        foreach (Transform t in transform)
        {
            var tileScript = t.GetComponent<TileBase>();

            if (tileScript.WillDie())
            {
                tileScript.Move();
            }
        }
    }

    private void TakeStep()
    {
        var killList = new List<TileBase>();
        var bornList = new List<TileBase>();

        //// Determine which cells live and which cell Dies in the next step (generation)
        foreach (Transform t in transform)
        {                    
            var tileScript = t.GetComponent<TileBase>();          

            if(tileScript.WillDie())
                killList.Add(tileScript);
            
            if(tileScript.WillBeBorn())
                bornList.Add(tileScript);
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
