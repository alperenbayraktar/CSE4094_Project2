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

    private void Awake()
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

        TableNumberI(TableCount, CellCount);
        CellNumberI(TableCount, CellCount);

        TableNumberInput.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            TableCount = int.Parse(value);
            TableNumberI(TableCount, CellCount);
        });
        CellNumberInput.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            CellCount = int.Parse(value);
            CellNumberI(TableCount, CellCount);
        });
        WordInput.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(value)) return;
            word = value;
            InsertB();
        });
    }

    public void InsertB()
    {
        EventManager.SendEvent(EventName.INSERT_CLICKED);
    }
    public void DeleteB()
    {

    }
    public void ResetB()
    {

    }
    public void TableNumberI(int TableCount, int CellCount)
    {
        EventManager.SendEvent(EventName.CREATE_TABLES);
        SetTablesActive(TableCount, CellCount);
        SetTablesInactive(5 - TableCount);
    }
    public void CellNumberI(int TableCount, int CellCount)
    {
        EventManager.SendEvent(EventName.CREATE_TABLES);
        SetCellsActive(CellCount, TableCount);
        SetCellsInactive(30 - CellCount, 5);
    }
    public void WordI()
    {

    }
    public void SetTablesInactive(int count)
    {
        foreach (List<Transform> cells in cellList.GetRange(5 - count, count))
        {
            print(cells[0].parent.name);
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
    }
}
