using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using Unity.EditorCoroutines.Editor;
using System;

public class CsvLoader : EditorWindow
{
    string commonURL1 = "https://docs.google.com/spreadsheets/d/e/";
    string sheetURL = "2PACX-1vSK6TMslr4fXQkwTIYxILzJRbfFS9GH3E4kerAHKYAdbR0ulnF3gwfAly-jYSjuqql9QZ7FHArWHBpX"; // この部分だけシートごとに書き換える。 ファイル > ウェブに公開
    string commonURL2 = "/pub?gid=";
    string commonURL3 = "&single=true&output=csv";

    public static Dictionary<string, string> fileList = new Dictionary<string, string>();
    public static Dictionary<string, bool> checkFileList = new Dictionary<string, bool>();
    public static bool isSelectCheck = false;


    [MenuItem("Window/CsvLoader")]
    static void Open()
    {
        var window = GetWindow<CsvLoader>();
        window.titleContent = new GUIContent("Csv Loader");
        LoadFileList();
    }

    static private void LoadFileList()
    {
        fileList.Clear();
        fileList.Add("opening.csv", "449411701");
        fileList.Add("opening2.csv", "906228607");
        fileList.Add("start_tutorial.csv", "314693860");
        fileList.Add("menu_tutorial.csv", "780507114");
        fileList.Add("switch_tutorial.csv", "1207200502");
        fileList.Add("stage1_boss.csv", "1829844643");
        fileList.Add("stage1_boss_result.csv", "1777858682");
        fileList.Add("stage2_boss.csv", "956662429");
        fileList.Add("stage2_boss_result.csv", "1860829014");
        fileList.Add("stage2_movie.csv", "11724559");
        fileList.Add("stage3_start.csv", "1673465631");
        fileList.Add("stage3_boss.csv", "992445580");
        fileList.Add("stage3_boss_result.csv", "1987249178");
        fileList.Add("stage4_boss.csv", "2069169908");
        fileList.Add("stage4_boss_result.csv", "538322999");
        fileList.Add("stage5_boss.csv", "437719464");
        fileList.Add("stage5_boss_result.csv", "1872305858");
        fileList.Add("stage6_boss_yukari.csv", "1981528181");
        fileList.Add("stage6_boss_maki.csv", "726849357");
        
        isSelectCheck  = true;
        checkFileList.Clear();
        foreach (var file in fileList)
        {
            checkFileList.Add(file.Key, false);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("Box");

        if (fileList.Count == 0)
        {
            LoadFileList();
        }

        if (GUILayout.Button("ファイルリストの更新"))
        {
            LoadFileList();
        }

        isSelectCheck  = EditorGUILayout.BeginToggleGroup("ファイルを選択する (オフのときは全部ダウンロード)", isSelectCheck );
        foreach (var file in fileList)
        {
            checkFileList[file.Key] = GUILayout.Toggle(checkFileList[file.Key], file.Key);
        }
        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("ダウンロード"))
        {
            Debug.Log("ダウンロード開始");
            foreach (var checkFile in checkFileList)
            {
                if (checkFile.Value || !isSelectCheck)
                {
                    EditorCoroutineUtility.StartCoroutine(Download(checkFile.Key, fileList[checkFile.Key]), this);
                }
            }
        }

        EditorGUILayout.EndVertical();

    }
    
    private IEnumerator Download(string fileName, string gid)
    {
        string filePath = Application.dataPath + "/Resources/Scenario/" + fileName;
        string url = commonURL1 + sheetURL + commonURL2 + gid + commonURL3 + "&v=" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        Debug.Log(filePath + " Loading...");
        Debug.Log("URL: " + url);
        using (var request = UnityWebRequest.Get(url)) {

            yield return request.SendWebRequest();

            if (request.error != null || request.isHttpError || request.isNetworkError)
            {
                yield break;
            }
        
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(request.downloadHandler.text);
            }
        }
        Debug.Log("<color=green>" + filePath + " Loaded!</color>");
    }
}
