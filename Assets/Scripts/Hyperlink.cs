using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyperlink : MonoBehaviour
{
    public string url = "https://www.instagram.com/nickjones.uk";

    public void OpenUrl()
    {
        Application.OpenURL(url);
    }
}
