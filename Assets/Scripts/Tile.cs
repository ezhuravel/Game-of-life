using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour
{
    private bool alive = false;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Die();
    }


    public bool IsAlive()
    {        
        return alive;
    }

    public void Die()
    {
        alive = false;
        sr.color = Color.white;
    }

    public void Born()
    {
        alive = true;
        sr.color = Color.blue;
    }

    /// <summary>
    /// Uses a collider to return the number adjacent live cells
    /// </summary>
    /// <returns>Number of Adjacent live cells</returns>
    public int AdjacentLiveCells()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);
        return collisions.Count(x => x.gameObject != gameObject && x.gameObject.GetComponent<Tile>().IsAlive()); ;
    }
}
