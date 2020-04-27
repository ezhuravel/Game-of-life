using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTile : TileBase
{
    public override void Move()
    {
        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(4, 4), 0);
       

        foreach (var tile in collisions)
        {
            var tileScript = tile.GetComponent<TileBase>();
            var adjacentLiveCellCount = tileScript.AdjacentLiveCells();

            if (tile.gameObject != gameObject && tileScript.tileState == TileState.Dead && !tileScript.Moved && adjacentLiveCellCount == 3)
            {
                Die();
                tileScript.Born();
                tileScript.Moved = true;

                break; // leave loop after move               
            }
            else if(tile.gameObject != gameObject && tileScript.tileState == TileState.Dead && !tileScript.Moved && adjacentLiveCellCount >= 2 && adjacentLiveCellCount < 4)
            {
                Die();
                tileScript.Born();
                tileScript.Moved = true;
            }
        }
    }
}
