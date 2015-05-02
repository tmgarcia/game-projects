using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextHelper : MonoBehaviour 
{
    private Text textUI;

	void Start () 
    {
        textUI = GetComponent<Text>();
	}
    
    public void setText(float v)
    {
        textUI.text = "" + v;
    }
    public void setTextTruncated(float v)
    {
        textUI.text = "" + ((int)v);
    }
    public void setText(int i)
    {
        textUI.text = "" + i;
    }
    public void setText(char c)
    {
        textUI.text = "" + c;
    }
    
    public void appendToText(float v)
    {
        textUI.text = textUI.text + "" + v;
    }
    public void appendToText(int i)
    {
        textUI.text = textUI.text + "" + i;
    }
    public void appendToText(char c)
    {
        textUI.text = textUI.text + "" + c;
    }
    public void appendToText(string s)
    {
        textUI.text = textUI.text + "" + s;
    }
}
