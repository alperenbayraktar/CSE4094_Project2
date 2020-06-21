using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuckooHashing : MonoBehaviour, IEventManagerListener
{
    public int TableSize;
    public List<string[]> Tables = new List<string[]>();
    public float sec;
    public static int tableIndex;
    public static int cellIndex;

    public Text LoopText;
    public Text InfoText;
    public InputField WaitSec;
    private void Awake()
    {
        EventManager.Subscribe(this);
    }
    private void Start()
    {
        SetLoopText(0, true);
        WaitSec.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            sec = float.Parse(value);
        });
    }
    private void CreateTables(int count, int size)
    {
        SetLoopText(stuck, true);
        Tables.Clear();
        if (count > 5) count = 5;
        if (count < 1) count = 1;
        if (size > 30) size = 30;
        if (size < 10) size = 10;
        for (int i = 0; i< count; i++)
        {
            string[] Table = new string[size];
            Tables.Add(Table);
        }
    }
    private int HashFunction(int table, string word)
    {
        int result = 0;
        int hash;
        var isNumeric = int.TryParse(word, out _);
        if (!isNumeric)
        {
            hash = word.GetHashCode();
        }
        else
        {
            hash = Int32.Parse(word);
        }
        switch (table)
        {
            case 0:                
                result = Math.Abs(hash % TableSize);
                break;
            case 1:
                result = Math.Abs(hash * 31 % TableSize);
                break;
            case 2:
                result = Math.Abs(hash * 83 % TableSize);
                break;
            case 3:
                result = Math.Abs((hash * 31 + 7) % TableSize);
                break;
            case 4:
                result = Math.Abs((hash * 83 + 31) % TableSize);
                break;
        }
        return result;
    }
    void SetStuff(int tI, int cI, string w)
    {
        Tables[tI][cI] = w;
        tableIndex = tI;
        cellIndex = cI;
        UIScript.word = w;
        InfoText.text = w + " has been inserted to table: " + tI + ", Cell: " + cI;
        EventManager.SendEvent(EventName.INSERT_DONE);
    }
    int stuck = 0;
    public void SetLoopText(int stuck, bool reset)
    {
        LoopText.text = "Loop: " + stuck + "/50 ";
        if (stuck == 50) LoopText.text += "\nInsert failed.";
        if (reset) LoopText.text = "Loop: 0/50 ";
    }
    IEnumerator insertT1(int index, string word)
    {
        if(Tables[0][index] == null)
        {
            SetLoopText(stuck, false);
            stuck = 0;
            SetStuff(0, index, word);
        }
        else
        {
            stuck++;
            SetLoopText(stuck, false);
            if (stuck == 50)
            {
                SetLoopText(stuck, false);
                stuck = 0;
                yield break;
            }
            string overwritedWord = Tables[0][index];            
            SetStuff(0, index, word);
            yield return new WaitForSeconds(sec);
            StartCoroutine(insertT2(HashFunction(1, overwritedWord), overwritedWord));
            
        }
    }
    IEnumerator insertT2(int index, string word)
    {
        if (Tables[1][index] == null)
        {
            SetLoopText(stuck, false);
            stuck = 0;
            SetStuff(1, index, word);
        }
        else
        {
            stuck++;
            SetLoopText(stuck, false);
            if (stuck == 50)
            {
                SetLoopText(stuck, false);
                stuck = 0;
                yield break;
            }
            if (Tables.Count > 2)
            {
                string overwritedWord = Tables[1][index];
                SetStuff(1, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT3(HashFunction(2, overwritedWord), overwritedWord));
            }
            else
            {
                string overwritedWord = Tables[1][index];
                SetStuff(1, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT1(HashFunction(0, overwritedWord), overwritedWord));
            }
        }       
    }
    IEnumerator insertT3(int index, string word)
    {
        if (Tables[2][index] == null)
        {
            SetLoopText(stuck, false);
            stuck = 0;
            SetStuff(2, index, word);
        }
        else
        {
            stuck++;
            SetLoopText(stuck, false);
            if (stuck == 50)
            {
                SetLoopText(stuck, false);                
                stuck = 0;
                yield break;
            }
            if (Tables.Count > 3)
            {
                string overwritedWord = Tables[2][index];
                SetStuff(2, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT4(HashFunction(3, overwritedWord), overwritedWord));
            }
            else
            {
                string overwritedWord = Tables[2][index];
                SetStuff(2, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT1(HashFunction(0, overwritedWord), overwritedWord));
            }
        }
    }
    IEnumerator insertT4(int index, string word)
    {
        if (Tables[3][index] == null)
        {
            SetLoopText(stuck, false);
            stuck = 0;
            SetStuff(3, index, word);
        }
        else
        {
            stuck++;
            SetLoopText(stuck, false);
            if (stuck == 50)
            {
                SetLoopText(stuck, false);
                stuck = 0;
                yield break;
            }
            if (Tables.Count > 4)
            {
                string overwritedWord = Tables[3][index];
                SetStuff(3, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT5(HashFunction(4, overwritedWord), overwritedWord));                
            }
            else
            {
                string overwritedWord = Tables[3][index];
                SetStuff(3, index, word);
                yield return new WaitForSeconds(sec);
                StartCoroutine(insertT1(HashFunction(0, overwritedWord), overwritedWord));
            }
        }
    }
    IEnumerator insertT5(int index, string word)
    {
        if (Tables[4][index] == null)
        {
            SetLoopText(stuck, false);
            stuck = 0;            
            SetStuff(4, index, word);
        }
        else
        {
            stuck++;
            SetLoopText(stuck, false);
            if (stuck == 50)
            {
                SetLoopText(stuck, false);
                stuck = 0;
                yield break;
            }
            yield return new WaitForSeconds(sec);
            string overwritedWord = Tables[4][index];
            SetStuff(4, index, word);
            StartCoroutine(insertT1(HashFunction(0, overwritedWord), overwritedWord));            
        }
    }
    public static int searchTableIndex;
    public static int searchCellIndex;
    private bool searh(string word)
    {
        int i = 0;
        bool flag = false;
        foreach(string[] table in Tables)
        {
            int index = HashFunction(i, word);
            if (string.Compare(table[index], word) == 0)
            {
                flag = true;
                InfoText.text = "Word is in Table: " + i + ", Cell: " + index;
                searchTableIndex = i;
                searchCellIndex = index;                
            }
            i++;
        }
        if (!flag)
        {
            InfoText.text = "Could not find " + word;
        }
        return flag;
    }

    private void DeleteCell(int tableIndex, int cellIndex)
    {
        Tables[tableIndex][cellIndex] = null;
        print(tableIndex + ", " + cellIndex);
        EventManager.SendEvent(EventName.DELETE_DONE);
    }

    void IEventManagerListener.OnEventRecieved(string eventName)
    {
        if (EventName.INSERT_CLICKED == eventName)
        {
            string word = UIScript.word;
            if (!searh(word))
            {
                StartCoroutine(insertT1(HashFunction(0, word), word));
            }            
        }
        if (EventName.CREATE_TABLES == eventName)
        {
            TableSize = UIScript.CellCount;
            CreateTables(UIScript.TableCount, UIScript.CellCount);
        }
        if(EventName.DELETE_CLICKED == eventName)
        {
            string word = UIScript.word;
            if (searh(word))
            {
                DeleteCell(searchTableIndex, searchCellIndex);
            }
            else
            {
                InfoText.text = "Item does not exist.";
            }
        }
        if(EventName.SEARCH_ON == eventName)
        {
            string word = UIScript.word;
            if (searh(word))
            {
                EventManager.SendEvent(EventName.CHANGE_COLOR);
            }
        }
    }
}
