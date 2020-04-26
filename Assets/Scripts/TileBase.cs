using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileBase : MonoBehaviour
{
    private SpriteRenderer sr;

    public bool Moved { get; set; }

    /// <summary>
    /// Initializes title
    /// </summary>
    private void Start()
    {
        Moved = false;
        sr = GetComponent<SpriteRenderer>();
        if (Alive)
            Born();
        else
            Die();
    }

    /// <summary>
    /// Returns current tiles living status 
    /// </summary>
    /// <returns></returns>
    public bool Alive { get; set; }


    /// <summary>
    /// Sets tile to dead
    /// </summary>
    public void Die()
    {
        Alive = false;
        sr.color = Color.white;
    }

    /// <summary>
    /// Sets tile to alive
    /// </summary>
    public void Born()
    {
        Alive = true;
        sr.color = Color.blue;
    }

    /// <summary>
    /// Uses a collider to return the number adjacent live cells
    /// </summary>
    /// <returns>Number of Adjacent live cells</returns>
    public int AdjacentLiveCells()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);
        return collisions.Count(x => x.gameObject != gameObject && x.gameObject.GetComponent<TileBase>().Alive);
    }

    /// <summary>
    /// Determines if the tile will die next generation
    /// </summary>
    /// <returns>Whether the tile will die or not</returns>
    public bool WillDie()
    {
        var adjacentLiveCellCount = AdjacentLiveCells();

        return Alive && (adjacentLiveCellCount < 2 || adjacentLiveCellCount > 3);
    }

    /// <summary>
    /// Determines if tile will be born
    /// </summary>
    /// <returns></returns>
    public bool WillBeBorn()
    {
        var adjacentLiveCellCount = AdjacentLiveCells();

        return !Alive && (adjacentLiveCellCount == 3);
    }

    public abstract void Move();
}
