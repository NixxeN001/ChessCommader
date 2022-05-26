using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject GameRulesGO;
    [SerializeField] GameObject DifficultySettingsGO;
    [SerializeField] GameObject MainMenuGO;
    // Start is called before the first frame update

    void Start()
    {
        MainMenuGO.SetActive(true);
        DifficultySettingsGO.SetActive(false);
        GameRulesGO.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EasyDifficulty()
    {

    }

    public void MediumDifficulty()
    {

    }

    public void HardDifficulty()
    {

    }

    public void Play()
    {
        MainMenuGO.SetActive(false);
        DifficultySettingsGO.SetActive(true);
    }
    public void GameRules()
    {
        MainMenuGO.SetActive(false);
        GameRulesGO.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameRulesBack()
    {
        GameRulesGO.SetActive(false);
        MainMenuGO.SetActive(true);
    }
    public void DifficultyBack()
    {
        DifficultySettingsGO.SetActive(false);
        MainMenuGO.SetActive(true);
    }
}
