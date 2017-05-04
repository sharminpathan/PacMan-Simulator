using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManAstar : MonoBehaviour
{
    public float speed = 4.0f;
    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    public Nodes currentNode;
    private Nodes previousNode, targetNode;
    public Sprite idleSprite;
    private GameObject pacMan;
    private int AstarCount = 0;
    object[] objects;
    object[] nodes;
    private int count = 201;

    // Use this for initialization
    void Start()
    {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        objects = GameObject.FindObjectsOfType(typeof(GameObject));
        nodes = GameObject.FindGameObjectsWithTag("PelletNodes");
        Nodes node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            currentNode = node;
        }
        //Debug.Log(currentNode);
        direction = Vector2.right;
        ChangePosition(direction);
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        Astar();
        Move();
        UpdateOrientation();
        UpdateAnimationState();
        ConsumePallet();
    }

    void checkInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
        }

    }

    void Astar()
    {
        Vector2 targetTile = Vector2.zero;
        Vector2 directionToSend = Vector2.zero;
        if (AstarCount < 1)
        {
            Nodes node = GetNodeAtPosition(transform.localPosition);
            if (node != null)
            {
                currentNode = node;
                targetNode = node;
                AstarCount++;
            }
        }

        if (transform.localPosition == targetNode.transform.position || currentNode != null)
        {
            currentNode = targetNode;
            Nodes moveToNode = null;
            Vector2 pacmanPosition = pacMan.transform.position;
            Vector2 currentTile = new Vector2(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));

            Nodes[] foundNodes = new Nodes[4];
            Vector2[] foundNodesDirection = new Vector2[4];

            int nodeCounter = 0;
            // Debug.Log(currentNode);
            for (int i = 0; i < currentNode.neighbors.Length; i++)
            {
                if (currentNode.validDirections[i] != direction * -1)
                {
                    foundNodes[nodeCounter] = currentNode.neighbors[i];

                    foundNodesDirection[nodeCounter] = currentNode.validDirections[i];
                    nodeCounter++;
                    //Debug.Log(currentNode.validDirections[i]);
                }
            }

            if (foundNodes.Length == 1)
            {
                moveToNode = foundNodes[0];
                directionToSend = foundNodesDirection[0];
            }
            if (foundNodes.Length > 1)
            {
                float leastDistance = 100000f;
                for (int i = 0; i < foundNodes.Length; i++)
                {
                    if (foundNodesDirection[i] != Vector2.zero && foundNodes[i]!=previousNode && foundNodes[i].tag != "Hiddens")
                    {
                        GameObject Pellet = GetTileAtPosition(foundNodes[i].transform.position);
                        Tile PelletTile = Pellet.GetComponent<Tile>();
                        if (!PelletTile.didConsume)
                        {
                            float distance = GetDistance(foundNodes[i].transform.position, currentTile);
                            if (distance < leastDistance)
                            {
                                leastDistance = distance;
                                moveToNode = foundNodes[i];
                                directionToSend = foundNodesDirection[i];
                            }
                        }
                        /*else
                        {   for (int j = 0; j < foundNodes[i].neighbors.Length; j++)
                            {
                                float distanceNeigbors = GetDistance(foundNodes[i].neighbors[j].transform.position, currentTile);
                                if (distanceNeigbors < leastDistance)
                                {
                                    leastDistance = distanceNeigbors;
                                    moveToNode = foundNodes[i].neighbors[j];
                                    directionToSend = foundNodes[i].neighbors[j].validDirections[j];
                                }
                            }
                        }*/
                    }
                }
            }
            if (moveToNode == null)
            {
                Tile PelletTile;
                Vector2 PelletTilePosition = Vector2.zero;

                foreach (GameObject o in nodes)
                {

                    //if (o.name != "PacMan" && o.name != "ghost" && o.name != "NonNodes" && o.name != "Nodes" && o.name != "Maze" && o.name != "Pellets") {
                    //if (o.transform.parent.name == "Nodes") {

                    //Pellet = GetTileAtPosition (o.transform.position);
                    PelletTile = o.GetComponent<Tile>();

                    if (!PelletTile.didConsume)
                    {
                        
                        PelletTilePosition = new Vector2(Mathf.RoundToInt(o.transform.position.x), Mathf.RoundToInt(o.transform.position.y));
                        break;
                    }


                    //}
                    //}
                }
                
                    Nodes[] foundNodes2 = new Nodes[4];
                    Vector2[] foundNodesDirection2 = new Vector2[4];

                    int nodeCounter2 = 0;

                    for (int i = 0; i < currentNode.neighbors.Length; i++)
                    {
                        if (currentNode.validDirections[i] != direction * (-1))
                        {
                            foundNodes2[nodeCounter2] = currentNode.neighbors[i];

                            foundNodesDirection2[nodeCounter2] = currentNode.validDirections[i];
                            nodeCounter2++;
                            // Debug.Log(currentNode.validDirections[i]);
                        }
                    }

                    if (foundNodes2.Length == 1)
                    {
                        moveToNode = foundNodes2[0];
                        directionToSend = foundNodesDirection2[0];
                    }
                    if (foundNodes2.Length > 1)
                    {
                        float leastDistance = 100000f;
                        for (int i = 0; i < foundNodes2.Length; i++)
                        {
                            if (foundNodesDirection2[i] != Vector2.zero && foundNodes2[i].tag!="Hiddens")
                            {
                                float distance = GetDistance(foundNodes2[i].transform.position, PelletTilePosition);
                                if (distance < leastDistance)
                                {
                                    leastDistance = distance;
                                    moveToNode = foundNodes2[i];
                                    directionToSend = foundNodesDirection2[i];
                                }
                            }
                        }
                    }
            }

            if (directionToSend.x == -1)
            {
                ChangePosition(Vector2.left);
            }
            else if (directionToSend.x == 1)
            {
                ChangePosition(Vector2.right);
            }
            else if (directionToSend.y == 1)
            {
                ChangePosition(Vector2.up);
            }
            else if (directionToSend.y == -1)
            {
                ChangePosition(Vector2.down);
            }
        }
    }

    void ChangePosition(Vector2 d)
    {
        if (d != direction)
        {
            nextDirection = d;
        }
        if (currentNode != null)
        {
            Nodes moveToNode = CanMove(d);
            if (moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    Nodes CanMove(Vector2 d)
    {
        Nodes moveToNode = null;
        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] == d)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }
        return moveToNode;
    }

    void Move()
    {

        if (targetNode != currentNode && targetNode != null)
        {
            if (nextDirection == direction * -1)
            {
                direction *= -1;
                Nodes tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }
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

                Nodes moveToNode = CanMove(nextDirection);

                if (moveToNode != null)
                    direction = nextDirection;

                if (moveToNode == null)
                    moveToNode = CanMove(direction);

                if (moveToNode != null)
                {
                    /* targetNode = moveToNode;
                     previousNode = currentNode;
                     currentNode = null;
                     */
                    Astar();
                }
                else
                {
                    direction = Vector2.zero;
                }

            }
            else
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
        }
    }

    void MoveToNode(Vector2 d)
    {
        Nodes moveToNode = CanMove(d);
        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        else if (direction == Vector2.right)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.up)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }


    Nodes GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<Nodes>();
        }
        else
        {
            return null;
        }
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }
    float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    void UpdateAnimationState()
    {
        if (direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
            GetComponent<Animator>().enabled = true;
    }

    void ConsumePallet()
    {
        GameObject o = GetTileAtPosition(transform.position);
        if (o != null)
        {
            Tile tile = o.GetComponent<Tile>();
            if (tile != null)
            {
                if (!tile.didConsume && (tile.isPellet || tile.isSuperPellet))
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;
                    tile.didConsume = true;
					count = count - 1;

					if (count == 0) {
						Application.Quit();
						UnityEditor.EditorApplication.isPlaying = false;
					}
                }
            }
        }
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX, tileY];

        if (tile != null)
            return tile;
        else
            return null;
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                    return otherPortal;
                }
            }
        }
        return null;
    }
    
    float GetDistance(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }
}