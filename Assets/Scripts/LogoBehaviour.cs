using System;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

public class LogoBehaviour : MonoBehaviour
{
    [SerializeField] private InputActionAsset asset;
    [SerializeField] private string levelName;
    
    private InputAction action;
    // Start is called before the first frame update
    void Start()
    {
        EnableInputs();
    }
    
    private void EnableInputs()
    {
        var actionMap = asset.GetActionMap("Player");
        action = actionMap.GetAction("Degroup");
        action.Enable();
        action.performed += OnSpace;
    }

    private void OnSpace(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(levelName);
    }

    private void OnDestroy()
    {
        action.performed -= OnSpace;
    }
}
