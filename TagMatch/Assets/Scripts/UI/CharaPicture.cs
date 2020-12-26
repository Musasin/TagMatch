using UnityEngine;
using UnityEditor;

public class CharaPicture
{
    public GraphDiffer main;
    public GraphDiffer eyeBrows;
    public GraphDiffer eye;
    public GraphDiffer mouse;
    public GraphDiffer option1;
    public GraphDiffer option2;
    public GraphDiffer option3;

    public CharaPicture(GameObject charaObject)
    {
        main = charaObject.GetComponent<GraphDiffer>();
        eyeBrows = charaObject.transform.Find("EyeBrows").GetComponent<GraphDiffer>();
        eye = charaObject.transform.Find("Eye").GetComponent<GraphDiffer>();
        mouse = charaObject.transform.Find("Mouse").GetComponent<GraphDiffer>();
        option1 = charaObject.transform.Find("Option1").GetComponent<GraphDiffer>();
        Transform opt2 = charaObject.transform.Find("Option2");
        if (opt2 != null)
            option2 = opt2.GetComponent<GraphDiffer>();
        Transform opt3 = charaObject.transform.Find("Option3");
        if (opt3 != null)
            option3 = opt3.GetComponent<GraphDiffer>();
    }
    public void SetSprite(string _main, string _eyeBrows, string _eye = "", string _mouse = "", string _option1 = "", string _option2 = "", string _option3 = "")
    {
        if (_main.Length     > 0) { main.SetSprite(_main); }
        if (_eyeBrows.Length > 0) { eyeBrows.SetSprite(_eyeBrows); }
        if (_eye.Length      > 0) { eye.SetSprite(_eye); }
        if (_mouse.Length    > 0) { mouse.SetSprite(_mouse); }
        if (_option1.Length  > 0) { option1.SetSprite(_option1); }
        if (_option2.Length  > 0) { option2.SetSprite(_option2); }
        if (_option3.Length  > 0) { option3.SetSprite(_option3); }
    }
}