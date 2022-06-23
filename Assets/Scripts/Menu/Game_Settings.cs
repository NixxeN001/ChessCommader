using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Settings : MonoBehaviour
{
    public static Game_Settings instance;

    [SerializeField] Slider dif_Slider;
    [SerializeField] Toggle PvCPU_toggle;
    [SerializeField] Toggle trainCPU_toggle;

    public int diff_Setting = 2;
    public bool PvCPU = false;

    public bool liveTrainCPU = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        dif_Slider.onValueChanged.AddListener(ChangeDifficulty);
        PvCPU_toggle.onValueChanged.AddListener((arg ) =>
        {
            PvCPU = arg;
        });
        trainCPU_toggle.onValueChanged.AddListener((value) =>
        {
            liveTrainCPU = value;
        });

        DontDestroyOnLoad(gameObject);
    }


    private void ChangeDifficulty(float diff)
    {
        diff_Setting = (int)diff;
    }
}
