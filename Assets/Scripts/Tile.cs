using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour
{
    /// <summary>
    /// Uses a collider to return the number adjacent live cells
    /// </summary>
    /// <returns>Number of Adjacent live cells</returns>
    public int AdjacentLiveCells()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);
        var count =  collisions.Count(x => x.gameObject != gameObject && x.gameObject.CompareTag("Live"));

        return count;
    }
}
