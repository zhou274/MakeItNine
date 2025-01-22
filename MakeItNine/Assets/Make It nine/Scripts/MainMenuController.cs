using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    private enum Panel { Splash, MainMenu, DifficultyMenu, Game, Pause, GameOver }
    public TextMeshProUGUI[] gameTitle_Text;
    public GameObject[] splash_panel, menu_panel , difficulty_panel, game_panel, gamePause_panel, gameOver_panel;
    public Button[] play_mainButton, tutorial_mainButton, quit_mainButton;
    public Button[] easy_difficultyButton, normal_difficultyButton, hard_difficultyButton, back_difficultyButton;
    public Button[] menu_GameButton;
    public Button[] menu_GamePauseButton, resume_GamePauseButton, restart_GamePauseButton;
    public TextMeshProUGUI[] winMessage_gameOverText;
    public Button[] restart_gameOverButton, menu_gameOverButton;
    public AudioSource audioSource;

    private Panel currentPanel = Panel.MainMenu;
    private int savedOrientation;
    public GameController gameController;
    public Tutorial tutorial;

    public Camera mainCamera;
    public enum Orientation { None, Landscape, Portrait }
    public Orientation settedOrientation;

    void Awake() {
        //gameController = FindObjectOfType<GameController>();
        //tutorial = FindObjectOfType<Tutorial>();
        ButtonActions(0);
        ButtonActions(1);

        foreach (TextMeshProUGUI gameTitleText in gameTitle_Text) {
            gameTitleText.text = Configuration.gameTitle;
        }
        
        #if UNITY_EDITOR
        if(settedOrientation == Orientation.Portrait) {
            Screen.orientation = ScreenOrientation.Portrait;
            savedOrientation = 1;
        } else if (settedOrientation == Orientation.Landscape) {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            savedOrientation = 0;
        } else {
            if (mainCamera.aspect < 0.6f) { 
                if (Screen.orientation == ScreenOrientation.Portrait) {
                    Screen.orientation = ScreenOrientation.Portrait;
                    savedOrientation = 1;
                } else {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    savedOrientation = 0;
                }
            }
            else {
                savedOrientation = 0;
            }
        }
        #else       
        if (mainCamera.aspect < 0.6f) { 
            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
                //Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = false;
                savedOrientation = 1;
            } else if(Screen.orientation == ScreenOrientation.LandscapeLeft) {
                //Screen.orientation = ScreenOrientation.Landscape;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                savedOrientation = 0;
            } else {
                //Screen.orientation = ScreenOrientation.Landscape;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                savedOrientation = 0; 
            }
         } else {
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                savedOrientation = 0;
         }
        #endif

 
        if (PlayerPrefs.GetInt("Testing") == 1) {
            ChangePanel(savedOrientation, Panel.DifficultyMenu);
        } else {
            StartCoroutine(WaitingForSplashScreen());
        }
    }

    private IEnumerator WaitingForSplashScreen() {
        yield return new WaitForSeconds(0.1f);
        /* ChangePanel(savedOrientation, Panel.Splash);
         yield return new WaitForSeconds(5);
         Color textColor;
         for (float i = 1; i > 0f; i -= Time.deltaTime) {
             textColor = splash_panel[savedOrientation].transform.Find("Text").GetComponent<Text>().color;
             splash_panel[savedOrientation].transform.Find("Text").GetComponent<Text>().color = new Color(textColor.r, textColor.b, textColor.g, i);
         }
         for (float i = 1; i > 0f; i -= Time.deltaTime) {
             splash_panel[savedOrientation].GetComponent<Image>().color = new Color(1, 1, 1, i);
             splash_panel[savedOrientation].transform.Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, i);
             yield return null;
         }*/
        ChangePanel(savedOrientation, Panel.MainMenu);
        if(PlayerPrefs.GetInt("Tutorial") == 0){ 
            tutorial.ShowTutorial(savedOrientation);
            PlayerPrefs.SetInt("Tutorial", 1);
        }
    }


    public void GameOver(int difficulty, float time) {
        ChangePanel(savedOrientation, Panel.GameOver);
        string difficultyText = difficulty == 3 ? "困难": difficulty == 2 ? "正常" : "简单";
        winMessage_gameOverText[0].text = "恭喜你胜利了 " + difficultyText + " 消耗 " + Mathf.FloorToInt(time / 60) + " 分钟 " + Mathf.CeilToInt(time % 60) + " 秒";
        winMessage_gameOverText[1].text = "恭喜你胜利了 " + difficultyText + " 消耗 " + Mathf.FloorToInt(time / 60) + " 分钟 " + Mathf.CeilToInt(time % 60) + " 秒";
    }

    public void PauseGame() {
        ChangePanel(savedOrientation, Panel.Pause);
     }

    #region Button Actions according to their orientation
    void ButtonActions(int orientation) {
        #region Main Menu Panel
        play_mainButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.DifficultyMenu);
        });
        quit_mainButton[orientation].onClick.AddListener(() => {
            PlaySound();
            Application.Quit();
        });
        tutorial_mainButton[orientation].onClick.AddListener(() => {
            PlaySound();
            tutorial.ShowTutorial(orientation);
        });
        #endregion

        #region Difficulty Menu Panel
        easy_difficultyButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.StartGame(savedOrientation, 1);
        });
        normal_difficultyButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.StartGame(savedOrientation, 2);
        });
        hard_difficultyButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.StartGame(savedOrientation, 3);
        });
        back_difficultyButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.MainMenu);
        });
        #endregion

        #region Game Panel
        menu_GameButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Pause); 
            gameController.PauseGame();
        });
        #endregion

        #region GamePause Panel
        restart_GamePauseButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.RestartGame();
        });
        menu_GamePauseButton[orientation].onClick.AddListener(() => {
            PlaySound();
            gameController.OnGameEnd();
            ChangePanel(orientation, Panel.MainMenu);
        });
        resume_GamePauseButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.ResumeGame();
        });
        #endregion

        #region GameOver Panel
        restart_gameOverButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.Game);
            gameController.RestartGame();
        });
        menu_gameOverButton[orientation].onClick.AddListener(() => {
            PlaySound();
            ChangePanel(orientation, Panel.MainMenu);
        });
        #endregion
    }
    #endregion

    #region Change Panel Method
    private void ChangePanel(int orientation, Panel panel) {
        splash_panel[orientation].SetActive(false);
        menu_panel[orientation].SetActive(false);
        difficulty_panel[orientation].SetActive(false);
        game_panel[orientation].SetActive(false);
        gamePause_panel[orientation].SetActive(false);
        gameOver_panel[orientation].SetActive(false);
        
        switch (panel) {
            case Panel.Splash:
                splash_panel[orientation].SetActive(true);
                break;
            case Panel.MainMenu:
                menu_panel[orientation].SetActive(true);
                break;
            case Panel.DifficultyMenu:
                difficulty_panel[orientation].SetActive(true);
                break;
            case Panel.Game:
                game_panel[orientation].SetActive(true);
                break;
            case Panel.Pause:
                gamePause_panel[orientation].SetActive(true);
                break;
            case Panel.GameOver:
                gameOver_panel[orientation].SetActive(true);
                break;
        }
        currentPanel = panel;
    }
    #endregion

    #region Play Sound
    public void PlaySound() {
        audioSource.Play();
    }
    #endregion

}
