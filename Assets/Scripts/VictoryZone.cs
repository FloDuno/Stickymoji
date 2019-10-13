using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private string nextLevel;
    // Todo : Add it to actionMap
    [SerializeField] private InputAction changeLevelInput;
    
    private AsyncOperation levelLoaded;
    private bool canChangeScene;

    // Start is called before the first frame update
    // Todo : Instance prefab instead of making it disabled
    private void Start()
    {
        winScreen.SetActive(false);
        canChangeScene = false;
        changeLevelInput.Enable();
        changeLevelInput.performed += OnChangeLevelKey;
    }

    private void OnChangeLevelKey(InputAction.CallbackContext obj)
    {
        if (canChangeScene)
        {
            levelLoaded.allowSceneActivation = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        winScreen.SetActive(true);
        levelLoaded = SceneManager.LoadSceneAsync(nextLevel, LoadSceneMode.Single);
        levelLoaded.allowSceneActivation = false;
        StartCoroutine(CheckLoadingScene());
    }

    private IEnumerator CheckLoadingScene()
    {
        while (!levelLoaded.isDone)
        {
            // Check if the loading has finished
            if (levelLoaded.progress >= 0.9f)
            {
                canChangeScene = true;
            }

            yield return null;
        }
    }
}