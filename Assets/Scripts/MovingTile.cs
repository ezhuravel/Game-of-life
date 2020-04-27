using System.Linq;
using UnityEngine;

public class MovingTile : TileBase
{     
    public override void Move()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);

        foreach (var tile in collisions)
        {
            var tileScript = tile.GetComponent<TileBase>();
            var adjacentLiveCellCount = tileScript.AdjacentLiveCells();

            if (tile.gameObject != gameObject && tileScript.tileState == TileState.Dead && !tileScript.Moved && adjacentLiveCellCount >= 2 && adjacentLiveCellCount < 4)
            {
                Die();
                tileScript.Born();
                tileScript.Moved = true;

                break; // leave loop after move               
            }
        }
    }
}
