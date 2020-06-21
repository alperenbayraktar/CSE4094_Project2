using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIScript : MonoBehaviour, IEventManagerListener
{
    public Button InsertButton;
    public Button DeleteButton;
    public Button ResetButton;
    public InputField TableNumberInput;
    public InputField CellNumberInput;
    public InputField WordInput;
    public RectTransform Tables;
    public List<Transform> tablesList = new List<Transform>();
    public List<List<Transform>> cellList = new List<List<Transform>>();

    public static int TableCount = 2;
    public static int CellCount = 10;

    public static string word;
    public float sec;
    public Text InfoText;
    public void Awake()
    {
        EventManager.Subscribe(this);
    }
    void Start()
    {        
        for (int i = 0; i < 5; i++)
        {
            List<Transform> cells = new List<Transform>();
            tablesList.Add(Tables.GetChild(i));
            for (int j = 0; j < 30; j++)
            {
                cells.Add(tablesList[i].GetChild(j));
            }
            cellList.Add(cells);
        }

        TableNumberInput.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            TableCount = int.Parse(value);
            if (TableCount > 5)
            {
                TableCount = 5;
                InfoText.text = "Table Count set to 5.\nTable Count should be 1<x<5.";
            }
            else if (TableCount < 1)
            {
                TableCount = 1;
                InfoText.text = "Table Count set to 1.\nTable Count should be 1<x<5.";
            }
            else InfoText.text = "Table Count set to " + TableCount;
            TableNumberI(TableCount, CellCount, false);
        });
        CellNumberInput.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            CellCount = int.Parse(value);
            if (CellCount > 30)
            {
                CellCount = 30;
                InfoText.text = "Cell Count set to 30.\nCell Count should be 10<x<30.";
            }
            else if (CellCount < 10)
            {
                CellCount = 10;
                InfoText.text = "Cell Count set to 10.\nCell Count should be 10<x<30.";
            }
            else InfoText.text = "Cell Count set to " + CellCount;
            CellNumberI(TableCount, CellCount, false);
        });
        WordInput.onValueChanged.AddListener((value) =>
        {
            word = value;
            cellList[CuckooHashing.searchTableIndex][CuckooHashing.searchCellIndex].GetComponent<Image>().color = Color.white;
            if (string.IsNullOrEmpty(value)) return;
            WordI();
        });        
        TableNumberI(TableCount, CellCount, false);
        CellNumberI(TableCount, CellCount, false);
        InfoText.text = "";
    }

    public void InsertB()
    {        
        EventManager.SendEvent(EventName.INSERT_CLICKED);
    }
    public void DeleteB()
    {
        EventManager.SendEvent(EventName.DELETE_CLICKED);
    }
    public void ResetB(bool reset)
    {
        foreach(List<Transform> table in cellList)
        {
            foreach(Transform cell in table)
            {
                cell.GetChild(0).GetComponent<Text>().text = null;
            }
        }
        TableNumberI(TableCount, CellCount, true);
        CellNumberI(TableCount, CellCount, true);
        if(reset) InfoText.text = "Reset.";
    }
    public void TableNumberI(int TableCount, int CellCount, bool reset)
    {
        if(!reset) ResetB(false);        
        EventManager.SendEvent(EventName.CREATE_TABLES);
        SetTablesActive(TableCount, CellCount);
        SetTablesInactive(5 - TableCount);        
    }
    public void CellNumberI(int TableCount, int CellCount, bool reset)
    {
        if (!reset) ResetB(false);
        EventManager.SendEvent(EventName.CREATE_TABLES);
        SetCellsActive(CellCount, TableCount);
        SetCellsInactive(30 - CellCount, 5);
    }
    public void WordI()
    {
        EventManager.SendEvent(EventName.SEARCH_ON);
    }
    public void SetTablesInactive(int count)
    {
        foreach (List<Transform> cells in cellList.GetRange(5 - count, count))
        {            
            SetCellsInactive(30, count);
        }
        foreach (Transform table in tablesList.GetRange(5 - count, count))
        {
            table.gameObject.SetActive(false);
        }
    }
    public void SetCellsInactive(int count, int tableCount)
    {
        foreach (List<Transform> cells in cellList.GetRange(5 - tableCount, tableCount))
        {
            foreach (Transform cell in cells.GetRange(30 - count, count))
            {
                if (cell.GetComponentInChildren<Text>().text.Length > 0) cell.GetComponentInChildren<Text>().text.Remove(0);
                cell.gameObject.SetActive(false);
            }
        }
    }
    public void SetTablesActive(int tableCount, int cellCount)
    {
        foreach (Transform table in tablesList.GetRange(0, tableCount))
        {
            table.gameObject.SetActive(true);
        }
        SetCellsActive(cellCount, tableCount);
    }
    public void SetCellsActive(int count, int tableCount)
    {
        int tableIndex;
        for (tableIndex = 0; tableIndex < tableCount; tableIndex++)
        {
            foreach (Transform cell in cellList[tableIndex].GetRange(0, count))
            {
                cell.gameObject.SetActive(true);
            }
        }
    }
    void IEventManagerListener.OnEventRecieved(string eventName)
    {
        if (EventName.INSERT_DONE == eventName)
        {            
            cellList[CuckooHashing.tableIndex][CuckooHashing.cellIndex].GetChild(0).GetComponent<Text>().text = word;
            
        }
        if (EventName.DELETE_DONE == eventName)
        {
            InfoText.text = cellList[CuckooHashing.searchTableIndex][CuckooHashing.searchCellIndex].GetChild(0).GetComponent<Text>().text +
                " has been deleted from table: " + CuckooHashing.searchTableIndex + ", Cell: " + CuckooHashing.searchCellIndex;
            cellList[CuckooHashing.searchTableIndex][CuckooHashing.searchCellIndex].GetChild(0).GetComponent<Text>().text = null;            
        }
        if (EventName.CHANGE_COLOR == eventName)
        {
            InfoText.text = cellList[CuckooHashing.searchTableIndex][CuckooHashing.searchCellIndex].GetChild(0).GetComponent<Text>().text +
                " has been found in table: " + CuckooHashing.searchTableIndex + ", Cell: " + CuckooHashing.searchCellIndex;
            cellList[CuckooHashing.searchTableIndex][CuckooHashing.searchCellIndex].GetComponent<Image>().color = Color.green;
        }
    }
    IEnumerator Waiter(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
}
