using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour {
    private static DialogManager _instance;
    Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();
    List<Dialog> visibleDialogs = new List<Dialog>();

    private static DialogManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DialogManager();
            return _instance;
        }
    }

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(_instance);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
            HandleKeyPress(KeyCode.Escape);
        if (Input.GetKeyUp(KeyCode.Return))
            HandleKeyPress(KeyCode.Return);
        if (Input.GetKeyUp(KeyCode.Tab))
            HandleKeyPress(KeyCode.Tab);
	}

    private void HandleKeyPress(KeyCode k)
    {
        if (visibleDialogs.Count > 0)
        {
            Dialog top = visibleDialogs[0];
            if (top)
                top.HandleKeyPress(k);
        }
    }

    public static void RegisterDialog(string name, Dialog dialog)
    {
        Instance.dialogs.Add(name, dialog);
    }

    public static void Show(string name)
    {
        if (Instance.dialogs.ContainsKey(name))
        {
            Dialog d = _instance.dialogs[name];
            if (d)
            {
                d.Show();
                _instance.visibleDialogs.Insert(0, d);
            }
        }
    }

    public static void Hide(string name)
    {
        if (Instance.dialogs.ContainsKey(name))
        {
            Dialog d = _instance.dialogs[name];
            if (d)
            {
                d.Hide();
                _instance.visibleDialogs.Remove(d);
            }
        }
    }
}
