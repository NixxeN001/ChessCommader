using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject GameRulesGo;
    [SerializeField] GameObject DifficultySettingsGo;
    [SerializeField] GameObject MainMenuGo;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuGo.SetActive(true);
        GameRulesGo.SetActive(false);
        DifficultySettingsGo.SetActive(false);
    }
    public void GameRules()
    {
        MainMenuGo.SetActive(false);    //hides the main and shows the current
        GameRulesGo.SetActive(true);
    }

    public void DifficultySettings()
    {
        MainMenuGo.SetActive(false);    //hides the main and shows the current
        DifficultySettingsGo.SetActive(true);
    }
    public void BackGameRules()
    {
        GameRulesGo.SetActive(false);     //hides current and shows main
        MainMenuGo?.SetActive(true);
    }
    public void BackDifficulty()
    {
        DifficultySettingsGo.SetActive(false);     //hides current and shows main
        MainMenuGo?.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
