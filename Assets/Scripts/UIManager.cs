using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Static instance
    public static UIManager instance;
    //References
    public TMP_Text ammoText;
    public TMP_Text arMagText;
    public TMP_Text pistolMagText;
    public TMP_Text healthBoostsText;
    public TMP_Text KeyText;
    public Slider healthSlider;
    public Image fillHealthSlider;
    public GameObject crossHair;
    public GameObject instructionText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public GameObject pausePanel;
    public GameObject pauseBtn;

    public Color initialColor;
    public ParticleSystem winEffect;


    void Awake() 
    {
        if(instance!=this && instance!=null)
        {
            Destroy(this);
        }
        else
        {
            instance=this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initialColor=fillHealthSlider.color;
        crossHair.SetActive(false);
        instructionText.SetActive(false);

        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowInstruction()
    {
        StartCoroutine(Instructions());
    }

    public void Win()
    {
        winEffect.Play();
        gameOverText.text="You made it!!";
        gameOverPanel.SetActive(true);
        pauseBtn.SetActive(false);
    }
    public void Fail()
    {
        gameOverText.text="Learn from mistakes..Try Again!";
        gameOverPanel.SetActive(true);
        pauseBtn.SetActive(false);
    }
    public void PauseBtn()
    {
        pausePanel.SetActive(true);
        pauseBtn.SetActive(false);
        Time.timeScale=0f;
    }
    public void ResumeBtn()
    {
        pausePanel.SetActive(false);
        pauseBtn.SetActive(true);
        Time.timeScale=1f;
    }
    public void BackBtn()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("Home");
    }
    public void ReplayBtn()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitBtn()
    {
        Application.Quit();
    }
    IEnumerator Instructions()
    {
        instructionText.SetActive(true);
        yield return new WaitForSeconds(2f);
        instructionText.SetActive(false);
    }
}
