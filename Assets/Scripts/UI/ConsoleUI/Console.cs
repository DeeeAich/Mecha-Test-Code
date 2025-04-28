using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    public ConsoleButton symbol_1;
    public ConsoleButton symbol_2;
    public ConsoleButton symbol_3;
    public ConsoleButton symbol_4;
    public ConsoleButton symbol_5;
    public ConsoleButton symbol_6;
    public ConsoleButton symbol_7;
    public ConsoleButton symbol_8;
    public ConsoleButton symbol_9;
    public GameObject codeEntered;

    public string testCode;

    public void InputCode()
    {
        string inputCode =
            symbol_1.code.ToString()
            + symbol_2.code.ToString()
            + symbol_3.code.ToString()
            + symbol_4.code.ToString()
            + symbol_5.code.ToString()
            + symbol_6.code.ToString()
            + symbol_7.code.ToString()
            + symbol_8.code.ToString()
            + symbol_9.code.ToString();

        if (testCode == inputCode) 
        {
            codeEntered.SetActive(true);
        }
        else
        {
            Debug.Log(inputCode);
            symbol_1.ResetSymbol();
            symbol_2.ResetSymbol();
            symbol_3.ResetSymbol();
            symbol_4.ResetSymbol();
            symbol_5.ResetSymbol();
            symbol_6.ResetSymbol();
            symbol_7.ResetSymbol();
            symbol_8.ResetSymbol();
            symbol_9.ResetSymbol();
        }
    }


}
