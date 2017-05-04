using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour {

    public Nodes[] neighbors;
    public Vector2[] validDirections;
	// Use this for initialization
	void Start () {
        validDirections = new Vector2[neighbors.Length];
        for(int i=0; i < neighbors.Length; i++)
        {
            Nodes neighbor = neighbors [i];
            Vector2 tempVector = neighbor.transform.localPosition - transform.localPosition;
            validDirections[i] = tempVector.normalized;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
