using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startgame : MonoBehaviour
{
    public Slider slider;
    public Button start;
    public GameObject eventObj;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("gamedata").GetComponent<Mapsizeslider>().slider = slider;
        GameObject.DontDestroyOnLoad(this.eventObj);
        start.onClick.AddListener(jumptomap);
    }
    public void jumptomap()
    {
        SceneManager.LoadScene(1);
    }
    IEnumerator LoadScene()
    {
        animator.SetBool("fadein", true);
        animator.SetBool("fadeout", false);
        yield return new WaitForSeconds(1);
    }
    // Update is called once per frame
    void Update()
    {
    }
}
