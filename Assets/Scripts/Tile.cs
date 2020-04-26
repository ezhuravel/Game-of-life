using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour
{   
    private bool moved = false;
    private SpriteRenderer sr;

    /// <summary>
    /// Initializes title
    /// </summary>
    private void Start()
    {
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
        return collisions.Count(x => x.gameObject != gameObject && x.gameObject.GetComponent<Tile>().Alive);
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

    public bool HasMoved()
    {
        return moved;
    }

    public void Move()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);

        foreach (var tile in collisions)
        {
            var tileScript = tile.GetComponent<Tile>();
            var adjacentLiveCellCount = tileScript.AdjacentLiveCells();

            if (tile.gameObject != gameObject && !tileScript.Alive && !tileScript.HasMoved() && adjacentLiveCellCount >= 2 && adjacentLiveCellCount < 4)
            {
                Die();
                tileScript.Born();
                tileScript.moved = true;

                break; // leave loop after move               
            }
        }
    }

    public void ResetMove()
    {
        moved = false;
    }
}
