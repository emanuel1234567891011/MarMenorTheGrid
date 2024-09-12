using System.IO;
using UnityEngine;

public static class CSVManager
{
    static string reportDirectoryName = "Reports";
    static string reportFileName = "report.csv";
    static string reportSeperator = ",";
    static string[] reportHeaders = new string[] { "Drone Number", "Battery Capacity", "Velocity", "Waste Capacity", "Charge Speed", "Cost of Electricity" };

    #region Actions

    public static void AppendToReport(string[] strings)
    {
        VerifyDirectory();
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                    finalString += reportSeperator;

                finalString += strings[i];
            }
            sw.WriteLine(finalString);
        }
    }

    public static string[] GetReportEntries()
    {
        VerifyDirectory();
        VerifyFile();
        return File.ReadAllLines(GetFilePath());
    }

    public static void CreateReport()
    {
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                    finalString += reportSeperator;

                finalString += reportHeaders[i];
            }
            sw.WriteLine(finalString);
        }
    }

    public static void PrintReportPath()
    {
        Debug.Log("<b><color=orange>Report file found at:</color></b> " + GetDirectoryPath());
    }

    #endregion

    #region Queries

    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(GetDirectoryPath()))
            Directory.CreateDirectory(dir);
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
            CreateReport();
    }

    static string GetDirectoryPath()
    {
        return Application.persistentDataPath + "/" + reportDirectoryName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    #endregion
}
