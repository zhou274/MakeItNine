using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour {


    public AudioSource musicAudioSource, soundAudioSource;
    public Toggle[] musicToggle, soundToggle;
    public Button[] settingButton;
    public Animation[] settingAnimation;

    private bool settingOpen;

    private void Awake() {
        if (PlayerPrefs.GetInt("FirstTime") == 0) {
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.SetFloat("Music", 1);
            PlayerPrefs.SetFloat("Sound", 0.6f);
        }
    }

    private void Start() {
        int orientation = 0;

        if (Screen.orientation == ScreenOrientation.Portrait)  {
            orientation = 1;
        }

        soundAudioSource.volume = PlayerPrefs.GetFloat("Sound");
        soundToggle[orientation].isOn = PlayerPrefs.GetFloat("Sound") == 0 ? false : true;
        musicAudioSource.volume = PlayerPrefs.GetFloat("Music");
        musicToggle[orientation].isOn = PlayerPrefs.GetFloat("Music") == 0 ? false : true;

        settingButton[orientation].onClick.AddListener(() => {
            settingOpen = !settingOpen;
            if(settingOpen) settingAnimation[orientation].Play("setting_open_portrait"); 
            else settingAnimation[orientation].Play("setting_close_portrait");
        });

        soundToggle[orientation].onValueChanged.AddListener((isOn) => {
            PlayerPrefs.SetFloat("Sound", isOn == true ? 0.6f : 0);
            soundAudioSource.volume = isOn == true ? 0.6f : 0;
        });

        musicToggle[orientation].onValueChanged.AddListener((isOn) => {
            PlayerPrefs.SetFloat("Music", isOn == true ? 1 : 0);
            musicAudioSource.volume = isOn == true ? 1 : 0;
        });
    }
}
