using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class CsvDownloader : MonoBehaviour
{
    string commonURL1 = "https://docs.google.com/spreadsheets/d/e/";
    string sheetURL = "2PACX-1vSK6TMslr4fXQkwTIYxILzJRbfFS9GH3E4kerAHKYAdbR0ulnF3gwfAly-jYSjuqql9QZ7FHArWHBpX"; // この部分だけシートごとに書き換える。 ファイル > ウェブに公開
    string commonURL2 = "/pub?gid=";
    string commonURL3 = "&single=true&output=csv";

    public string gid;      // URLの後ろのgid=xxxxxxxxの部分
    public string filePath; // 書き出しファイルのパス Resourceからのパスで記載する

    private void Start(){
        StartCoroutine(Download(commonURL1 + sheetURL + commonURL2 + gid + commonURL3));
    }

    private IEnumerator Download(string url)
    {
        Debug.Log(filePath + " Loading...");
        Debug.Log("URL: " + commonURL1 + sheetURL + commonURL2 + gid + commonURL3);
        using (var request = UnityWebRequest.Get(url)) {

            yield return request.SendWebRequest();

            if (request.error != null || request.isHttpError || request.isNetworkError)
            {
                yield break;
            }
        
            using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/" + filePath + ".csv"))
            {
                sw.WriteLine(request.downloadHandler.text);
            }
        }
        Debug.Log(filePath + " Loaded!");
    }
}