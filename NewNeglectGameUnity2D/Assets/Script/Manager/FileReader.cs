using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class FileReader : MonoBehaviour
{
    //static string SPLIT_RE = ",";
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    #region CSVReader 함수
    public static List<Dictionary<string, object>> ReadCSVFile(string file)
    {
        var list = new List<Dictionary<string, object>>();
        string newPath = "Csv\\" + file;
        Debug.Log("FileReader Exist : " + newPath);
        TextAsset data = Resources.Load(newPath) as TextAsset;


        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
            return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            Debug.Log("FileReader reading : " + newPath + "  Value : "+value +"   finalValue : "+ finalvalue.ToString());
            }
            list.Add(entry);
        }
        return list;
    }
    #endregion

    public static List<Dictionary<string, object>> StreamReaderRead(string file)
    {
        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        Debug.Log("FileReader : " + Application.streamingAssetsPath + "\\Csv\\" + file + ".csv");
        string source;
        StreamReader sr = new StreamReader(Application.streamingAssetsPath+ "\\Csv\\" + file +".csv");
        source = sr.ReadToEnd();
        sr.Close();

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }










    #region TXTRedaer 함수
    public static string ReadTXTFile(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            value = reader.ReadToEnd();
            reader.Close();
        }

        else
            value = "파일이 없습니다.";

        return value;
    }
    #endregion

    #region TXTWriter 함수
    public static void WriteTXTFile(string filePath, string message)
    {
        FileStream f = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f, System.Text.Encoding.Unicode);
        writer.WriteLine(message);
        writer.Close();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
