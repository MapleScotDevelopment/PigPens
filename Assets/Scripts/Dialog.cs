using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Dialog : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    abstract public void Show();
    abstract public void Hide();

    public virtual void HandleKeyPress(KeyCode k) { }
}
