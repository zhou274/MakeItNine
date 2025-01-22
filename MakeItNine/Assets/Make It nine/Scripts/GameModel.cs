using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour {

    private int completionNumber = Configuration.makeItTo;

    private int row, column;
    private int[] numberList;
    private int numberOfMatches;
    private Dictionary<int, List<int>> matches = new Dictionary<int, List<int>>();
    private int numberOfUniqueMatches;
    private Dictionary<int, int> uniqueMatches = new Dictionary<int, int>();

    public int GetNumberOFUniqueMatches() {
        return numberOfUniqueMatches;
    }

    public Dictionary<int, List<int>> GetMatches() {
        return matches;
    }

    public Dictionary<int, int> GetUniqueMatches() {
        return uniqueMatches;
    }

    public int[] CreateList(int row, int column) {
        int totalNumbers = row * column;
        this.row = row;
        this.column = column;
        this.numberList = new int[totalNumbers];
        this.numberOfMatches = 0;
        this.matches = new Dictionary<int, List<int>>();
        this.numberOfUniqueMatches = 0;
        this.uniqueMatches = new Dictionary<int, int>();
        for (int i = 0; i < totalNumbers; i++) {
            numberList[i] = Random.Range(1, 9);
        }
        SetNumberOfMatches();
        SetNumberOfUniqueMatches();
        return numberList;
    }

    public bool CheckMatch(int firstId,int secondId) {
        if (matches.ContainsKey(firstId)) {
            if (matches[firstId].Exists(item => item == secondId)) {
                return true;
            }
        }
        if (matches.ContainsKey(secondId)) {
            if (matches[secondId].Exists(item => item == firstId)) {
                return true;
            }
        }
        return false;
    }

    private void SetNumberOfMatches() {
        this.numberOfMatches = 0;
        this.matches = new Dictionary<int, List<int>>();
        int totalNumbers = row * column;
        for (int id = 0; id < totalNumbers - 1; id++) { // Except last one
            if (id < totalNumbers - column) {               // Every Row Except Last 
                if (id == 0 || id % column == 0) {                // First Column
                    if (CheckForward(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottomRight(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottom(id)) {
                        numberOfMatches++;
                    }
                } else if ((id + 1) % column == 0) {             // Last Column
                    if (CheckBottom(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottomLeft(id)) {
                        numberOfMatches++;
                    }
                } else {                                        // Other than first and last
                    if (CheckForward(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottomRight(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottom(id)) {
                        numberOfMatches++;
                    }
                    if (CheckBottomLeft(id)) {
                        numberOfMatches++;
                    }
                }
            } else {                                    // Last Row
                if (CheckForward(id)) {
                    numberOfMatches++;
                }
            }
        }
    }

    private void SetNumberOfUniqueMatches() {
        this.numberOfUniqueMatches = 0;
        this.uniqueMatches = new Dictionary<int, int>();
        foreach (KeyValuePair<int, List<int>> valuePair in matches) {
            if (uniqueMatches.ContainsValue(valuePair.Key) || uniqueMatches.ContainsKey(valuePair.Key)) {
                continue;
            }
            int savedLastValue = -404;
            if (valuePair.Value.Count > 1) {
                for (int j = 0; j < valuePair.Value.Count; j++) {
                    if (uniqueMatches.ContainsValue(valuePair.Value[j]) || uniqueMatches.ContainsKey(valuePair.Value[j])) {
                        continue;
                    }
                    if (matches.ContainsKey(valuePair.Value[j])) {
                        savedLastValue = valuePair.Value[j];
                    } else {

                        uniqueMatches.Add(valuePair.Key, valuePair.Value[j]);
                        break;
                    }
                    if (j == valuePair.Value.Count - 1 && savedLastValue != -404) {
                        uniqueMatches.Add(valuePair.Key, savedLastValue);
                    }
                }
            } else {
                if (uniqueMatches.ContainsValue(valuePair.Value[0]) || uniqueMatches.ContainsKey(valuePair.Value[0])) {
                    continue;
                }
                uniqueMatches.Add(valuePair.Key, valuePair.Value[0]);
            }
        }
        numberOfUniqueMatches = uniqueMatches.Count;
    }

    private bool CheckForward(int id) {
        if (numberList[id] + numberList[id + 1] == completionNumber) {
            AddDataToDictionaryOfList(matches, id, id + 1);
            return true;
        }
        return false;
    }

    private bool CheckBottomRight(int id) {
        if (numberList[id] + numberList[id + column + 1] == completionNumber) {
            AddDataToDictionaryOfList(matches, id, id + column + 1);
            return true;
        }
        return false;
    }

    private bool CheckBottom(int id) {
        if (numberList[id] + numberList[id + column] == completionNumber) {
            AddDataToDictionaryOfList(matches, id, id + column);
            return true;
        }
        return false;
    }

    private bool CheckBottomLeft(int id) {
        if (numberList[id] + numberList[id + column - 1] == completionNumber) {
            AddDataToDictionaryOfList(matches, id, id + column - 1);
            return true;
        }
        return false;
    }

    private void AddDataToDictionaryOfList(Dictionary<int, List<int>> dic, int key, int value) {
        if (dic.ContainsKey(key)) {
            dic[key].Add(value);
        } else {
            List<int> list = new List<int>();
            list.Add(value);
            matches.Add(key, list);
        }
    }
}