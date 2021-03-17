using UnityEngine;
using System;
using System.Text;
using System.Net;
using UnityEngine.UI;
using System.IO;

public class WebViewScript : MonoBehaviour
{
    public Text msgInfo;

    // Must be change it with your information.
    const string clientID = "2OezmFJLKiE3nsxa";         // client_id

    const string authServer = "https://auth.play2bit.com";
    const string authPath = "/v1/oauth/authorize";      // request authorized code

    const string sampleServer = "http://dev-auth-sample-web.play2bit.com";
    const string callbackPath = "/oauth/callback";      // for redirect_uri
    const string tokenPath = "/oauth/token";            // request user access_token
    const string shellInfoPath = "/app/token/account";  // request user shell info

    // const string loginUrl = "https://accounts.play2bit.com/member/login";
  
    string token;
    string access_token;
    int chkSeq;

    private WebViewObject webViewObject = null;

    [Serializable]
    class UserData
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public int expires_in;
        public string scope;
        public int app_no;
        public int member_no;
        public string nickname;
    }

    [Serializable]
    class ShellData
    {
        public string token_addr;
        public int amt_token;
        public int status;
    }

    [Serializable]
    class ErrorData
    {
        public int code;
        public string message;
    }

    [Serializable]
    class ShellInfo
    {
        public int status;
        public ShellData data;
        public ErrorData error;
    }

    // Use this for initialization
    void Start()
    {
        token = "";
        access_token = "";
        chkSeq = 0;
    }

    // Update is called once per frame
    // 모바일 디바이스를 백 버튼을 통해서 WebView 를 닫기 위한 코드
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                msgInfo.text = "";
                if (webViewObject != null) Destroy(webViewObject);
                return;
            }
        }
    }

    public void clearMsgInfo()
    {
        msgInfo.text = "User Info Region\r\n";
    }

    public void ReqLogin()
    {
        string authResponseType = "code";
        string authstate = "123";
        string tmpReqLogin = "";

        if (webViewObject == null)
        {
            tmpReqLogin += "[ReqLogin] webViewObject NOT exist!!!";
        }
        else
        {
            tmpReqLogin += "[ReqLogin] webViewObject Exist";
            // Destroy(webViewObject);
            webViewObject.SetVisibility(false);
        }

        if (!string.IsNullOrEmpty(access_token))
        {
            msgInfo.text += tmpReqLogin + "\r\n";
            msgInfo.text += "[access_token] " + access_token + "\r\n";
            return;
        };

        string reqAuthorizedCodeUrl = string.Format("{0}{1}?response_type={2}&client_id={3}&state={4}&redirect_uri={5}{6}"
            , authServer
            , authPath
            , authResponseType
            , clientID
            , authstate
            , sampleServer
            , callbackPath
            );

        msgInfo.text += tmpReqLogin + "\r\n";
        msgInfo.text += "[" + chkSeq + "] " + reqAuthorizedCodeUrl + "\r\n";

#if !UNITY_EDITOR && UNITY_ANDROID
        if (webViewObject == null)
        {
            webViewObject =
                (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

            msgInfo.text += "[ -- Create a webViewObject -- ]" + "\r\n";
        }

        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
                msgInfo.text += "[11] "+ msg + "\r\n"; 
                // msgInfo.text += "[21] " +  webViewObject.GetCookies(loginUrl) + "\r\n";
            },
            err: (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                msgInfo.text += "[12] "+ msg + "\r\n"; 
                // msgInfo.text += "[22] " +  webViewObject.GetCookies(loginUrl) + "\r\n";
            },
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
                msgInfo.text += "[13] "+ msg + "\r\n"; 
                // msgInfo.text += "[23] " +  webViewObject.GetCookies(loginUrl) + "\r\n";

            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                msgInfo.text += "[14] "+ msg + "\r\n"; 
                // msgInfo.text += "[24] " +  webViewObject.GetCookies(loginUrl) + "\r\n";
        
                if (msg.Contains("?code=")) {
                    string temp = msg.Substring(msg.IndexOf("=") + 1);
                    token = temp.Split('&')[0];
                    HttpGetByWebRequ();

                    if (webViewObject != null) Destroy(webViewObject);
                    // if (webViewObject != null) webViewObject.SetVisibility(false);
                }
            },
            ua: "Mozilla/5.0 (Linux; Android 4.1.1; Galaxy Nexus Build/JRO03C) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19"
        );

        webViewObject.LoadURL(reqAuthorizedCodeUrl);
        webViewObject.SetVisibility(true);
        webViewObject.SetMargins(200, 500, 80, 100);
        // webViewObject.SetMargins(Screen.width / 7, Screen.height / 4, Screen.width / 15, Screen.height / 15);   // left, top, right, bottm

#elif UNITY_EDITOR

#endif
    }

    public void HttpGetByWebRequ()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sampleServer + tokenPath);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.Timeout = 30 * 1000;

        // POST할 데이타를 Request Stream에 쓴다
        byte[] bytes = Encoding.ASCII.GetBytes(string.Format("code={0}", token));
        request.ContentLength = bytes.Length; // 바이트수 지정

        // Get the request stream.  
        Stream dataStream = request.GetRequestStream();

        // Write the data to the request stream.  
        dataStream.Write(bytes, 0, bytes.Length);

        // Close the Stream object.  
        dataStream.Close();

        // Response 처리
        string responseText = string.Empty;

        using (WebResponse resp = request.GetResponse())
        {
            Stream respStream = resp.GetResponseStream();
            using (StreamReader sr = new StreamReader(respStream))
            {
                responseText = sr.ReadToEnd();
            }
        }

        ShowUserInfoData(responseText);

        Console.WriteLine(responseText);
    }

    void ShowUserInfoData(string dataText)
    {
        UserData userInfo = JsonUtility.FromJson<UserData>(dataText);
        access_token = userInfo.access_token;

        msgInfo.text += "<color=#8c8a9a>chkSeq</color>    " + chkSeq.ToString() + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Access_Token</color>    " + userInfo.access_token + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Token_type</color>    " + userInfo.token_type + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Refresh_token</color>    " + userInfo.refresh_token + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Expires_in</color>    " + userInfo.expires_in.ToString() + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Scope</color>    " + userInfo.scope + "\r\n";
        msgInfo.text += "<color=#8c8a9a>App_no</color>    " + userInfo.app_no.ToString() + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Member_no</color>    " + userInfo.member_no.ToString() + "\r\n";
        msgInfo.text += "<color=#8c8a9a>Nickname</color>    " + userInfo.nickname + "\r\n";
    }

    public void ReqShellInfo()
    {
        msgInfo.text += "[2] Shell Info";
        msgInfo.text += "[access_token] " + access_token;

        if (string.IsNullOrEmpty(access_token)) return;

        string responseText = string.Empty;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sampleServer + shellInfoPath);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.Timeout = 30 * 1000;

        // POST할 데이타를 Request Stream에 쓴다
        byte[] bytes = Encoding.ASCII.GetBytes(string.Format("access_token={0}", access_token));
        request.ContentLength = bytes.Length; // 바이트수 지정

        // Get the request stream.  
        Stream dataStream = request.GetRequestStream();

        // Write the data to the request stream.  
        dataStream.Write(bytes, 0, bytes.Length);

        // Close the Stream object.  
        dataStream.Close();

        // Response 처리
        using (WebResponse resp = request.GetResponse())
        {
            Stream respStream = resp.GetResponseStream();
            using (StreamReader sr = new StreamReader(respStream))
            {
                responseText = sr.ReadToEnd();
            }
        }

        ShowUserShellData(responseText);

        Console.WriteLine(responseText);
    }

    void ShowUserShellData(string dataText)
    {
        ShellInfo shellInfo = JsonUtility.FromJson<ShellInfo>(dataText);

        msgInfo.text += "\r\n";
        msgInfo.text += "<color=#8c8a9a>Status</color>    " + shellInfo.status.ToString() + "\r\n";
        msgInfo.text += "<color=#8c8a9a>chkSeq</color>    " + chkSeq.ToString() + "\r\n";

        if (shellInfo.status == 200)
        {
            msgInfo.text += "<color=#8c8a9a>Token_addr</color>    " + shellInfo.data.token_addr + "\r\n";
            msgInfo.text += "<color=#8c8a9a>Amt_token</color>    " + shellInfo.data.amt_token.ToString() + "\r\n";
            msgInfo.text += "<color=#8c8a9a>Status</color>    " + shellInfo.data.status.ToString() + "\r\n";
        }
        else
        {
            msgInfo.text += "<color=#8c8a9a>Code</color>    " + shellInfo.error.code.ToString() + "\r\n";
            msgInfo.text += "<color=#8c8a9a>Message</color>    " + shellInfo.error.message + "\r\n";
        }
    }
    public void ClearMsgInfo()
    {
        if (webViewObject != null) webViewObject.SetVisibility(false);
    }

    public void ClearCookies()
    {
        webViewObject.ClearCookies();
    }
    public static string Encode(string text)
    {
        byte[] plainText = Encoding.UTF8.GetBytes(text);
        byte[] cipherText = plainText;

        return Convert.ToBase64String(cipherText);
    }

    public static string Decode(string text)
    {
        byte[] cipherText = Convert.FromBase64String(text);
        byte[] plainText = cipherText;

        return Encoding.UTF8.GetString(plainText);
    }
}
