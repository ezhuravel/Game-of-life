using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool alive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        var cellSprite = gameObject.GetComponent<SpriteRenderer>();
        if (alive)
        {
            alive = false;
            cellSprite.color = Color.white;
        }
        else
        {
            alive = true;
            cellSprite.color = Color.red;
        }


        var collisions = Physics2D.OverlapBoxAll(gameObject.transform.position, new Vector2(2, 2), 0);

        foreach (var collision in collisions)
        {
            if(collision.gameObject != gameObject)
            {
                SpriteRenderer spriteRenderer = null;

                if (collision.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                {
                    spriteRenderer.color = Color.green;
                }
            }           
        }
    }
}
