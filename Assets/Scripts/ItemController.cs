using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Text numberText;

    private int number;
    private int x, y;
    private int fieldNumber = -1;
    private bool emptyField = false;
    private GameController gameController;

    public bool Empty
    {
        get { return emptyField; }
        set { emptyField = value; }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    public void SetNumber(int num)
    {
        number = num;
        numberText.text = number.ToString();
    }

    public void SetPosition(int _x, int _y)
    {
        X = _x;
        Y = _y;
    }

    public void SetEmpty()
    {
        name = "Empty";
        emptyField = true;
        numberText.enabled = false;
        GetComponent<Image>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public bool OwnPosition()
    {
        return gameController.GetField(this) == number;
    }

    private void OnMouseUpAsButton()
    {
        gameController.ChangeItemPosition(this);
    }
}