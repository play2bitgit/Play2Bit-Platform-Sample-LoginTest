using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleManager : MonoBehaviour
{
    public WebViewScript WebScript;
    
    public void OnClickLogin()
    {
        WebScript.ReqLogin();
    }

    public void OnClickShellInfo()
    {
        WebScript.ReqShellInfo();
    }

    public void OnClickCloseApp()
    {
        //WebScript.ClearCookies();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void OnClickInit()
    {
        //WebScript.init();
    }

    public void OnClickClearMsgInfo()
    {
        WebScript.clearMsgInfo();
    }
}
