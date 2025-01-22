using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    public Button[] skipButton;
    public GameObject[] tutorialPanel;
    public MainMenuController menuController;
    //private int savedOrientation;

    private void Start() {
        skipButton[0].onClick.AddListener(() => {
            menuController.PlaySound();
            tutorialPanel[0].SetActive(false);
        });
        skipButton[1].onClick.AddListener(() => {
            menuController.PlaySound();
            tutorialPanel[1].SetActive(false);
        });
    }

    public void ShowTutorial(int orientation) {
        tutorialPanel[orientation].SetActive(true);
    }
}
