﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    public float moveSpeed= 3.9f;
    public Nodes startingPosition;
    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;
    public int chaseModeTimer4 = 20;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

	private int first = 0;

    public enum Mode
    {
        Chase,Scatter,Frightened
    }

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    private GameObject pacMan;
    private Nodes currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    // Use this for initialization
    void Start () {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        Nodes node = GetNodeAtPosition(transform.localPosition);
        if(node != null)
        {
            currentNode = node;
        }
        direction = Vector2.right;
        previousNode = currentNode;
        Vector2 pacmanPosition = pacMan.transform.position;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));
        targetNode = GetNodeAtPosition(targetTile);
	}
	
	// Update is called once per frame
	void Update () {
        ModeUpdate();
        Move();
	}

    void Move()
    {
        if (targetNode != currentNode && targetNode !=null)
        {
            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;
                GameObject otherPortal = GetPortal(currentNode.transform.position);
                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Nodes>();
                }
                targetNode = chooseNextNode();
                previousNode = currentNode;
                currentNode = null;
            }
            else
            {
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void ModeUpdate()
    {

        if (currentMode != Mode.Frightened) {
            modeChangeTimer += Time.deltaTime;
            if (modeChangeIteration == 1)
            {
                if(currentMode==Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    //Debug.Log("1 scatter");
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if(currentMode==Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer=0;
                }
            }
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    //Debug.Log("2 scatter");
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {   
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    //Debug.Log("3 scatter");
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {

                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    //Debug.Log("4 scatter");
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
        else if (currentMode == Mode.Frightened)
        {

        }
    }

    //----------------------------------Most Important Method--------------------------------------------------
    Nodes chooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;

        Vector2 pacmanPosition = pacMan.transform.position;
        targetTile = new Vector2(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));

        Nodes moveToNode = null;

        Nodes[] foundNodes = new Nodes[4];
        Vector2[] foundNodesDirection = new Vector2[4];

        int nodeCounter = 0;

        for(int i=0; i < currentNode.neighbors.Length; i++)
        {
			if(currentNode.validDirections[i] != direction *(-1))
            {
                foundNodes[nodeCounter] = currentNode.neighbors[i];
        
                foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                nodeCounter ++;
               // Debug.Log(currentNode.validDirections[i]);
            }
        }
		//Debug.Log (foundNodes.Length);
		//if (foundNodes.Length == 1) {
			moveToNode = foundNodes [0];
			direction = foundNodesDirection [0];
			

        //}
        //if (foundNodes.Length > 1)
			
            /*float leastDistance = 0;
            for(int i=0; i < foundNodes.Length; i++)
            {
                if (foundNodesDirection[i] != Vector2.zero)
                {
                    float distance = GetDistance(foundNodes[i].transform.position, targetTile);
                    if (distance > leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }*/
        //}
        return moveToNode;
    }

    void ChangeMode(Mode m)
    {
        currentMode = m;
    }

    Nodes GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            if (tile.GetComponent<Nodes>() != null)
            {
                return tile.GetComponent<Nodes>();
            }
        }
        return null;
    }

    GameObject GetPortal (Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if(tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal;
            }
        }
        return null;
    }

    float LengthFromNode (Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float GetDistance (Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }
}
