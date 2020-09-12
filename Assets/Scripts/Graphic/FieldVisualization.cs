using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldVisualization : MonoBehaviour
{
    public Game gameScript;
    public GameObject[] minoes;
    public GameObject fieldColour;
    // Start is called before the first frame update
    void Start()
    {
        gameScript = GameObject.Find("Main Camera").GetComponent<Game>();
                GameObject f = GameObject.Instantiate(fieldColour, new Vector3(0, 0, 0), Quaternion.identity);

                f.transform.parent = gameObject.transform;

                f.transform.localPosition = new Vector3(4.5f, 9.5f, 0);

                f.transform.parent = null;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!gameScript.gameover)
        {
            foreach (Transform child in gameObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            for (int i = 19; i >= 0; i--)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (gameScript.field[i, j] > 0 && gameScript.field[i, j] < 20)
                    {

                        GameObject tile = GameObject.Instantiate(minoes[gameScript.field[i, j] - 1], new Vector3(0, 0, 0), Quaternion.identity);
                        tile.transform.parent = gameObject.transform;
                        tile.transform.localPosition = new Vector3(j, i, 0);

                        //Sprite.Create(minoes[gameScript.field[i, j] - 1], new Rect(0,0,64,64),new Vector2(64*i,64*j),100); ;
                        //tileMap.SetTile(new Vector3Int(i,j,0), null);
                    }
                    if (gameScript.field[i, j] < 0)
                    {

                        GameObject tile = GameObject.Instantiate(minoes[-gameScript.field[i, j] - 1], new Vector3(0, 0, 0), Quaternion.identity);
                        ;
                        tile.transform.parent = gameObject.transform;

                        tile.transform.localPosition = new Vector3(j, i, 0);
                    }
                    if (gameScript.field[i, j] > 20)
                    {

                        GameObject tile = GameObject.Instantiate(minoes[gameScript.field[i, j] - 21], new Vector3(0, 0, 0), Quaternion.identity);
                        ;
                        tile.transform.parent = gameObject.transform;

                        tile.transform.localPosition = new Vector3(j, i, 0);
                    }

                }
            }
        }
    }
}
