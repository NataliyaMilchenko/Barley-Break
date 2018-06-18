using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum Side
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class GameController : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform parent;
    public Vector3 startPos;
    public int distance = 100;
    public float speed = 1;

    public static bool playing { get; private set; }

    private readonly int fieldCount = 24;
    private readonly int size = 5;

    private int moves = 0;
    public int Moves
    {
        get { return moves; }
        set
        {
            moves = value;
            if (OnMovesChanged != null)
                OnMovesChanged(moves);
        }
    }

    private List<ItemController> items = new List<ItemController>();
    private ItemController emptyItem;
    private bool active = false;

    public event Action<int> OnMovesChanged;
    public event Action OnGameStart;
    public event Action OnGameFinished;

    void Start()
    {
        CreateField();
        StartCoroutine(Shuffle());
    }

    void CreateField()
    {
        for (int i = 0; i < fieldCount + 1; i++)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(parent);
            item.transform.localScale = Vector3.one;

            var controller = item.GetComponent<ItemController>();
            controller.SetGameController(this);
            controller.SetNumber(i + 1);
            items.Add(controller);
        }

        emptyItem = items[items.Count - 1];
        emptyItem.SetEmpty();
    }

    public IEnumerator Shuffle()
    {
        var x = startPos.x;
        var y = startPos.y;
        items = items.OrderBy(item => UnityEngine.Random.value).ToList();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                items[i * size + j].transform.localPosition = new Vector3(x, y);
                items[i * size + j].SetPosition(j + 1, i + 1);

                x += distance;
            }
            x = startPos.x;
            y -= distance;
        }
        playing = true;
        yield return null;

        Clear();
        if (OnGameStart != null)
            OnGameStart();
    }
    void Clear()
    {
        Moves = 0;
    }

    bool CheckWin()
    {
        foreach (var item in items)
        {
            if (!item.OwnPosition())
                return false;
        }
        playing = false;
        Debug.Log("Win");
        return true;
    }

    private Side GetSide(ItemController current)
    {
        var empty = items.Find(x => x.Empty);
        if (current.X - 1 == empty.X && current.Y == empty.Y)
            return Side.Left;
        else if (current.X + 1 == empty.X && current.Y == empty.Y)
            return Side.Right;
        else if (current.Y - 1 == empty.Y && current.X == empty.X)
            return Side.Up;
        else if (current.Y + 1 == empty.Y && current.X == empty.X)
            return Side.Down;
        return Side.None;
    }

    public int GetField(ItemController item)
    {
        int i = (item.Y - 1) * size + item.X;
        return i;
    }

    private void ChangeFieldPosition(ItemController item)
    {
        var tempX = item.X;
        var tempY = item.Y;
        item.X = emptyItem.X;
        item.Y = emptyItem.Y;
        emptyItem.X = tempX;
        emptyItem.Y = tempY;
    }

    public void ChangeItemPosition(ItemController item)
    {
        if (!playing || active)
            return;

        Side side = GetSide(item);
        if (side != Side.None)
        {
            ChangeFieldPosition(item);
            active = true;
            StartCoroutine(MoveToSide(item, GetEndPosition(item, side)));
        }
    }

    IEnumerator MoveToSide(ItemController item, Vector3 endPos)
    {
        float t = 0;
        Vector3 startPos = item.transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            item.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Moves++;

        if (CheckWin())
        {
            if (OnGameFinished != null)
                OnGameFinished();
        }
        yield return null;
        active = false;
    }

    Vector3 GetEndPosition(ItemController item, Side side)
    {
        float endX = 0, endY = 0;
        switch (side)
        {
            case Side.Up:
                endX = item.transform.localPosition.x;
                endY = item.transform.localPosition.y + distance;
                break;
            case Side.Down:
                endX = item.transform.localPosition.x;
                endY = item.transform.localPosition.y - distance;
                break;
            case Side.Left:
                endX = item.transform.localPosition.x - distance;
                endY = item.transform.localPosition.y;
                break;
            case Side.Right:
                endX = item.transform.localPosition.x + distance;
                endY = item.transform.localPosition.y;
                break;
        }
        return new Vector3(endX, endY);
    }
}