using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerColor playerColor;
    public TMP_InputField inputField;
    public Toggle toggle;

	// Use this for initialization
	void Start () {
        playerColor = (PlayerColor)int.Parse(gameObject.name.Substring(6));
        print(" Player is " + playerColor.ToString() + "\n");
        GameController.addPlayer(this);
	}

    public void OnName(string ignored)
    {
        if (!string.IsNullOrEmpty(inputField.text))
            toggle.isOn = true;
    }
}
