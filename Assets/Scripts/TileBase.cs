using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileBase : MonoBehaviour
{
    private SpriteRenderer sr;
    public TileState tileState { get; set; }

    public bool Moved { get; set; }

    /// <summary>
    /// Initializes title
    /// </summary>
    private void Start()
    {
        Moved = false;
        sr = GetComponent<SpriteRenderer>();
        if (tileState == TileState.Alive)
            Born();
        else if (tileState == TileState.Barrier)
            SetBarrier();
        else
            Die();
    }

    /// <summary>
    /// Sets tile to dead
    /// </summary>
    public void Die()
    {
        tileState = TileState.Dead;
        sr.color = Color.white;
    }

    /// <summary>
    /// Sets tile to alive
    /// </summary>
    public void Born()
    {
        tileState = TileState.Alive;
        sr.color = Color.blue;
    }

    public void SetBarrier()
    {
        tileState = TileState.Barrier;
        sr.color = Color.red;
    }

    /// <summary>
    /// Uses a collider to return the number adjacent live cells
    /// </summary>
    /// <returns>Number of Adjacent live cells</returns>
    public int AdjacentLiveCells()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);
        return collisions.Count(x => x.gameObject != gameObject && x.gameObject.GetComponent<TileBase>().tileState == TileState.Alive);
    }

    public int AdjacentBarrierCell()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);        
        return collisions.Count(x => x.gameObject != gameObject && x.gameObject.GetComponent<TileBase>().tileState == TileState.Barrier);
    }

    /// <summary>
    /// Determines if the tile will die next generation
    /// </summary>
    /// <returns>Whether the tile will die or not</returns>
    public bool WillDie()
    {
        var adjacentLiveCellCount = AdjacentLiveCells();

        return tileState == TileState.Alive && (adjacentLiveCellCount < 2 || adjacentLiveCellCount > 3);
    }

    /// <summary>
    /// Determines if tile will be born
    /// </summary>
    /// <returns></returns>
    public bool WillBeBorn()
    {
        var adjacentLiveCellCount = AdjacentLiveCells();

        return tileState == TileState.Dead && (adjacentLiveCellCount == 3);
    }

    public abstract void Move();
}
