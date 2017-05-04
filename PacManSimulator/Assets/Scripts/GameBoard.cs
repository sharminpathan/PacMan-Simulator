using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    private static int boardWidth = 28;
    private static int boardHeigth = 36;

    public GameObject[,] board = new GameObject[boardWidth, boardHeigth];
    // Use this for initialization
    void Start () {
        object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            if(o.name != "PacMan" && o.name != "NonNodes" && o.name != "Nodes" && o.name != "Maze" && o.name!="Pellets")
            {
                board[(int)pos.x, (int)pos.y] = o;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
