using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class GameController : MonoBehaviour {

    #region References
    public Button[] hintButton;
    public GameObject[] buttonContainer;
    public TextMeshProUGUI[] timeText;
    public TextMeshProUGUI[] remainingText;
    public GameModel gameModel;
    public MainMenuController menuController;
    public Camera mainCamera;
    #endregion

    #region Button Creation Varriables
    private GameObject button_prefab;
    private int id_lastButtonPressed;
    private GameObject gameObject_lastButtonPressed;
    private bool removePress = Configuration.doublePress;
    private float buttonInitialPosition;
    #endregion

    private int savedOrientation;
    private int savedDifficulty;
    private int numberOfFoundMatches;
    private float timeOfGamePlay = 0;
    private bool gameIsPlaying = false;
    public string clickid;
    private StarkAdManager starkAdManager;
    private void Start() {
        PlayerPrefs.SetInt("Testing", 0);
        //gameModel = FindObjectOfType<GameModel>();
        //menuController = FindObjectOfType<MainMenuController>();
        hintButton[0].onClick.AddListener(() => {
            GameHintOn();
        });
        hintButton[1].onClick.AddListener(() => {
            GameHintOn();
        });
    }

    private void Update() {
        if (gameIsPlaying) {
            timeOfGamePlay += Time.deltaTime;
            timeText[0].text = "时间: " + Mathf.CeilToInt(timeOfGamePlay);
            timeText[1].text = "时间 " + Mathf.CeilToInt(timeOfGamePlay);
        }
    }

    public void StartGame(int orientation, int difficulty) {
        savedOrientation = orientation;
        savedDifficulty = difficulty;
        timeOfGamePlay = 0;
        gameIsPlaying = true;
        numberOfFoundMatches = 0;

        if (button_prefab == null) {
            button_prefab = buttonContainer[orientation].transform.Find("Button").gameObject;
            buttonInitialPosition = button_prefab.GetComponent<RectTransform>().anchoredPosition.x;
        } else {
            button_prefab.SetActive(true);
        }
        if (orientation == 1) {
            int column = 5;
            int row = 3;            // int maxRow = 9;
            Debug.Log(mainCamera.aspect);
            if (difficulty == 3) {
                if (mainCamera.aspect < 0.5) {
                    row = 9;
                } else if (mainCamera.aspect < 0.6) {
                    row = 8;
                } else {
                    row = 5;
                }
            } else if (difficulty == 2) {
                row = 6;
            }
            CreatesButton(button_prefab, buttonContainer[orientation].transform, row, column);
        } else {
            int row = 5;
            int column = 5;         //int maxColumn = 15;
            if (difficulty == 3) {
                column = 15;
                Vector3 position = button_prefab.GetComponent<RectTransform>().anchoredPosition;
                button_prefab.GetComponent<RectTransform>().anchoredPosition = new Vector3(buttonInitialPosition, position.y, position.z) ;
            } else if (difficulty == 2) {
                column = 10;
                float width = button_prefab.GetComponent<RectTransform>().sizeDelta.x;
                float spacing = width / 4;
                Vector3 position = button_prefab.GetComponent<RectTransform>().anchoredPosition;
                position = new Vector3(buttonInitialPosition + (spacing * 2.5f) + (width * 2.5f), position.y, position.z);
                button_prefab.GetComponent<RectTransform>().anchoredPosition = position;
            } else {
                float width = button_prefab.GetComponent<RectTransform>().sizeDelta.x;
                float spacing = width/4;
                Vector3 position = button_prefab.GetComponent<RectTransform>().anchoredPosition;
                position = new Vector3(buttonInitialPosition + (spacing * 5) + (width * 5), position.y, position.z);
                button_prefab.GetComponent<RectTransform>().anchoredPosition = position;
            }
            CreatesButton(button_prefab, buttonContainer[orientation].transform, row, column);
        }
        button_prefab.SetActive(false);

        remainingText[0].text = "剩余: " + (gameModel.GetNumberOFUniqueMatches() - 1);
        remainingText[1].text = "剩余: " + (gameModel.GetNumberOFUniqueMatches() - 1);
    }

    public void ResumeGame() {
        gameIsPlaying = true;
    }

    public void PauseGame() {
        gameIsPlaying = false;
    }

    public void OnGameEnd() {
        gameIsPlaying = false;
        for (int i = 0; i < buttonContainer[savedOrientation].transform.childCount; i++) {
            if(buttonContainer[savedOrientation].transform.GetChild(i).name == "Button") {
                continue;
            }
            Destroy(buttonContainer[savedOrientation].transform.GetChild(i).gameObject);
        }
    }

    private void EndGame() {
        OnGameEnd();
        menuController.GameOver(savedDifficulty,timeOfGamePlay);
        ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
    private void GameHintOn() {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    if (numberOfFoundMatches == 0)
                    {
                        ShowMinmumMatches(buttonContainer[savedOrientation].transform);
                    }
                    else
                    {
                        ShowAllMatches(buttonContainer[savedOrientation].transform);
                    }


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }

    public void RestartGame() {
        OnGameEnd();
        StartGame(savedOrientation, savedDifficulty);
    }

    #region Game Hint
    private void ShowAllMatches(Transform buttonContainer)
    {
        /*float blueColor = 0.9f;
        float redColor = 0.9f;
        float greenColor = 0.9f;*/
        foreach (KeyValuePair<int, List<int>> pair in gameModel.GetMatches()) {
            //Debug.Log("Match" + pair.Key + ":" + pair.Value);
            /*buttonContainer.Find(pair.Key.ToString()).GetComponent<Image>().color = new Color(redColor, greenColor, blueColor, 0.2f);
            foreach (int value in pair.Value) {
                buttonContainer.Find(value.ToString()).GetComponent<Image>().color = new Color(redColor, greenColor, blueColor, 0.2f);
            }
            blueColor += 0.01f;
            redColor += 0.02f;
            greenColor += 0.03f;*/
            buttonContainer.Find(pair.Key.ToString()).GetComponent<Animation>().Play();
            buttonContainer.Find(pair.Key.ToString()).GetComponent<CustomButton>().isFoundInMatches = true;
            foreach (int value in pair.Value) { 
                buttonContainer.Find(value.ToString()).GetComponent<Animation>().Play();
                buttonContainer.Find(value.ToString()).GetComponent<CustomButton>().isFoundInMatches = true;
            }
        }
    }

    private void ShowMinmumMatches(Transform buttonContainer) {
        /*float blueColor = 0.9f;
        float redColor = 0.9f;
        float greenColor = 0.9f;*/
        foreach (KeyValuePair<int, int> pair in gameModel.GetUniqueMatches()) {
            //Debug.Log("Match" + pair.Key + ":" + pair.Value);
            /*buttonContainer.Find(pair.Key.ToString()).GetComponent<Image>().color = new Color(redColor, greenColor, blueColor, 0.2f);
            buttonContainer.Find(pair.Value.ToString()).GetComponent<Image>().color = new Color(redColor, greenColor, blueColor, 0.2f);
            blueColor += 0.01f;
            redColor += 0.02f;
            greenColor += 0.03f;*/
            buttonContainer.Find(pair.Key.ToString()).GetComponent<Animation>().Play();
            buttonContainer.Find(pair.Value.ToString()).GetComponent<Animation>().Play();

            buttonContainer.Find(pair.Key.ToString()).GetComponent<CustomButton>().isFoundInMatches = true;
            buttonContainer.Find(pair.Value.ToString()).GetComponent<CustomButton>().isFoundInMatches = true;
        }
        /*float blueColor = 0.5f;
        float redColor = 0.5f;
        float greenColor = 0.5f;
        foreach (KeyValuePair<int, int> pair in gameModel.GetUniqueMatches()) {
            //Debug.Log("Match" + pair.Key + ":" + pair.Value);
            ColorBlock colorBlock = buttonContainer.Find(pair.Key.ToString()).GetComponent<Button>().colors;
            colorBlock.normalColor = new Color(redColor, greenColor, blueColor, 1);
            buttonContainer.Find(pair.Key.ToString()).GetComponent<Button>().colors = colorBlock;
            buttonContainer.Find(pair.Value.ToString()).GetComponent<Button>().colors = colorBlock;
            blueColor += 0.05f;
            redColor += 0.1f;
            greenColor += 0.15f;
        }*/
    }
    #endregion

    #region Create Button
    private void CreatesButton(GameObject button_prefab, Transform parentTransform, int row, int column) {
        int[] numberList = gameModel.CreateList(row, column);
        int currentId = 0;
        for (int currentRow = 0; currentRow < row; currentRow++) {
            for (int currentColumn = 0; currentColumn < column; currentColumn++) {
                float height = button_prefab.GetComponent<RectTransform>().sizeDelta.y;
                float width = button_prefab.GetComponent<RectTransform>().sizeDelta.x;
                float spacing = width/4;
                GameObject temp_button = Instantiate(button_prefab, parentTransform);
                Vector3 position = temp_button.GetComponent<RectTransform>().anchoredPosition;
                position = new Vector3(position.x + (width * currentColumn) + (spacing * currentColumn), -(position.y + (height * currentRow) + (spacing * currentRow)), position.z);
                temp_button.GetComponent<RectTransform>().anchoredPosition = position;
                temp_button.name = currentId.ToString();
                temp_button.transform.Find("Text").GetComponent<Text>().text = numberList[currentId].ToString();
                temp_button.GetComponent<Image>().color = new Color(Random.Range(0f, 0.9f), Random.Range(0f, 0.9f), Random.Range(0f, 0.9f), 1);
                temp_button.transform.Find("Text").GetComponent<Text>().color = new Color(0.74f, 0.82f, 0.94f,1);
                temp_button.GetComponent<Button>().onClick.AddListener(() => {
                    ButtonCLick(int.Parse(temp_button.name), temp_button);
                });
                currentId++;
            }
        }
    }

    private void ButtonCLick(int number, GameObject button_gameObject) {
        menuController.PlaySound();
        //Debug.Log(number_lastButtonPressed + ":" +  number);
        if (gameObject_lastButtonPressed == null) {
            gameObject_lastButtonPressed = button_gameObject;
            if (gameObject_lastButtonPressed.GetComponent<CustomButton>().isFoundInMatches) {
                gameObject_lastButtonPressed.GetComponent<Animation>().Stop();
            }
            id_lastButtonPressed = number;
        } else {
            if (gameModel.CheckMatch(number, id_lastButtonPressed)) {
                gameObject_lastButtonPressed.SetActive(false);
                button_gameObject.SetActive(false);
                gameObject_lastButtonPressed = null;
                FoundMatch();
            } else {
                if (removePress) {
                    if (gameObject_lastButtonPressed.GetComponent<CustomButton>().isFoundInMatches) {
                        gameObject_lastButtonPressed.GetComponent<Animation>().Play();
                    }
                    gameObject_lastButtonPressed = null;
                    button_gameObject.GetComponent<Button>().enabled = true;

                } else { 
                    gameObject_lastButtonPressed = button_gameObject;
                    if (gameObject_lastButtonPressed.GetComponent<CustomButton>().isFoundInMatches) {
                        gameObject_lastButtonPressed.GetComponent<Animation>().Play();
                    }
                    id_lastButtonPressed = number;
                }
            }
        }
    }

    private void FoundMatch() {
        numberOfFoundMatches++;
        remainingText[0].text = "剩余: " + (gameModel.GetNumberOFUniqueMatches() - numberOfFoundMatches - 1);
        remainingText[1].text = "剩余: " + (gameModel.GetNumberOFUniqueMatches() - numberOfFoundMatches - 1);
        if (numberOfFoundMatches + 1 == gameModel.GetNumberOFUniqueMatches()) {
            EndGame();
        }
    }
    #endregion
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
