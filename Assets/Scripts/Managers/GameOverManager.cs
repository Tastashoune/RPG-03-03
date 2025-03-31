using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    /*
    public GameObject gameOver;
    public GameObject canvas1;
    public InventoryManager im;
    */
    void Start()
    {
        //gameOver.SetActive(false);
        //im.ToggleInventory();
        Debug.Log("Start OK");
    }
    public void ActiveGoPanel()
    {
        /*
        canvas1.SetActive(false);
        gameOver.SetActive(true);
        */
    }
    public void ReTry()
    {
        Debug.Log("ReTry OK");
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
