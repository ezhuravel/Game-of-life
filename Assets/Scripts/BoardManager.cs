﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    float elapsed = 0f;
    private bool paused = true;
    bool moveStep = true;
 
    void Start()
    {
        SetUpBoard();    

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

                var tileScript = emptyTile.GetComponent<Tile>();

                var val = r.Next(2);
                if (val == 1)
                    tileScript.SetAlive();

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
            
            MoveCells();
            TakeStep();
            
            //if (moveStep)
            //{
            //    MoveCells();
            //    moveStep = false;
            //}
            //else
            //{
            //    TakeStep();
            //    moveStep = true;
            //}            
        }
    }

    private void MoveCells()
    {
        foreach (Transform t in transform)
        {
            var tileScript = t.GetComponent<Tile>();

            if (tileScript.WillDie())
            {
                tileScript.Move();
            }
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
