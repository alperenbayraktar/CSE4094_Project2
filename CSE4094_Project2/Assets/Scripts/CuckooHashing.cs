﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuckooHashing : MonoBehaviour, IEventManagerListener
{
    public int TableSize;
    public List<string[]> Tables = new List<string[]>();

    public static int tableIndex;
    public static int cellIndex;

    private void Awake()
    {
        EventManager.Subscribe(this);
    }
    private void CreateTables(int count, int size)
    {
        Tables.Clear();
        if (count > 5) count = 5;
        if (count < 1) count = 1;
        if (size > 30) size = 30;
        if (size < 1) size = 10;
        for (int i = 0; i< count; i++)
        {
            string[] Table = new string[size];
            Tables.Add(Table);
        }
    }
    private int HashFunction(int table, string word)
    {
        int result = 0;
        int hash = word.GetHashCode();
        switch (table)
        {
            case 1:                
                result = Math.Abs(hash % TableSize);
                break;
            case 2:
                result = Math.Abs(hash * 31 % TableSize);
                break;
            case 3:
                result = Math.Abs(hash * 83 % TableSize);
                break;
            case 4:
                result = Math.Abs((hash * 31 + 7) % TableSize);
                break;
            case 5:
                result = Math.Abs((hash * 83 + 31) % TableSize);
                break;
        }
        return result;
    }
    void SetStuff(int tI, int cI, string w)
    {
        tableIndex = tI;
        cellIndex = cI;
        UIScript.word = w;
        EventManager.SendEvent(EventName.INSERT_DONE);
    }
    public void insertT1(int index, string word)
    {
        if(Tables[0][index] == null)
        {
            Tables[0][index] = word;
            SetStuff(0, index, word);
        }
        else
        {
            insertT2(HashFunction(2, word), Tables[0][index]);
            Tables[0][index] = word;
            SetStuff(0, index, word);
        }
    }
    private void insertT2(int index, string word)
    {
        if (Tables[1][index] == null)
        {
            print(word + ", " + index);
            Tables[1][index] = word;
            SetStuff(1, index, word);
        }
        else
        {
            insertT3(HashFunction(3, word), word);
            Tables[1][index] = word;
            SetStuff(1, index, word);
        }       
    }
    private void insertT3(int index, string word)
    {
        if (Tables[2][index] == null)
        {
            Tables[2][index] = word;
            SetStuff(2, index, word);
        }
        else
        {
            insertT4(HashFunction(4, word), word);
            Tables[2][index] = word;
            SetStuff(2, index, word);
        }
    }
    private void insertT4(int index, string word)
    {
        if (Tables[3][index] == null)
        {
            Tables[3][index] = word;
            SetStuff(3, index, word);
        }
        else
        {
            insertT5(HashFunction(5, word), word);
            Tables[3][index] = word;
            SetStuff(3, index, word);
        }
    }
    private void insertT5(int index, string word)
    {
        if (Tables[4][index] == null)
        {
            Tables[4][index] = word;
            SetStuff(4, index, word);
        }
        else
        {
            insertT1(HashFunction(1, word), word);
            Tables[4][index] = word;
            SetStuff(4, index, word);
        }
    }

    private bool searh(string word)
    {
        int i = 1;
        bool flag = false;
        foreach(string[] table in Tables)
        {
            int index = HashFunction(i, word);
            if (string.Compare(table[index], word) == 0)
            {
                flag = true;
                Debug.Log("Word is in Table " + i);
                i++;
            }
        }
        if (!flag)
        {
            Debug.Log("Could not find.");
        }
        return flag;
    }

    void IEventManagerListener.OnEventRecieved(string eventName)
    {
        if (EventName.INSERT_CLICKED == eventName)
        {
            string word = UIScript.word;
            if (!searh(word))
            {
                insertT1(HashFunction(1, word), word);
            }            
        }
        if (EventName.CREATE_TABLES == eventName)
        {
            TableSize = UIScript.CellCount;
            CreateTables(UIScript.TableCount, UIScript.CellCount);
        }
    }
}