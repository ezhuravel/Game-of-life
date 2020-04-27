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
    public int seed;
    public bool paused = false;
    public Text StepText;
    public Text AliveCellText;
    public Text DeadCellText;
    public Text Ratio;
    
    float elapsed = 0f;
   
    private int step = 0;
    private float ratio = 0;
    List<float> ratiosList;
    private bool first = true;

    void Start()
    {
        //SetUpBoard();
        SetUpBoardWithCaves();
        ratiosList = new List<float>();

        Camera.orthographicSize = Columns / 2 ;

        float gridW = Columns * TileSize;
        float gridH = Rows * TileSize;

        transform.position = new Vector2(-gridW / 2 + TileSize / 2, gridH / 2 - TileSize / 2);
    }

    private void SetUpBoard()
    {
        System.Random r = new System.Random(2);

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
        RandomFillMap();       
    }

    private void SmoothMap()
    {
        var cells = GetComponentsInChildren<TileBase>();
        foreach (var cell in cells)
        {
            var tileScript = cell.GetComponent<TileBase>();
            int neighbourWallTiles = tileScript.AdjacentBarrierCell();

            if (neighbourWallTiles > 4)
                tileScript.SetBarrier();
            else if (neighbourWallTiles < 4)
                tileScript.Die();
        }
    }

    private void RandomFillMap()
    {
        System.Random r = new System.Random(2);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject emptyTile = Instantiate(CellGameObject, transform);
                emptyTile.name = $"Tile Row: {i} Column {j}";
                emptyTile.transform.position = new Vector2(j * TileSize, i * -TileSize);

                var tileScript = emptyTile.GetComponent<TileBase>();
                
                var val = r.Next(2);

                if (i == 0 || i == Rows - 1 || j == 0 || j == Rows - 1)
                {
                    tileScript.tileState = TileState.Barrier;
                }
                else
                {
                    tileScript.tileState = TileState.Dead;
                    if (r.Next(0, 100) < 45)
                    {
                        tileScript.tileState = TileState.Barrier;
                    }
                }

                //if (val == 1)
                //    tileScript.tileState = TileState.Alive;
                //else
                //    tileScript.tileState = TileState.Dead;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            for (int i = 0; i < 0; i++)
            {
                SmoothMap();
            }
            first = false;
        }


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

        if(step == 100)
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
            var deadCells = totalCells - liveCells;

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
