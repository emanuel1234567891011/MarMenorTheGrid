using System.IO;
using UnityEngine;

public static class CSVManager
{
    static string reportDirectoryName = "Reports";
    static string reportFileName = "report.csv";
    static string reportSeperator = ",";
    static string[] reportHeaders = new string[] { "Time", "Waste Collected (KG)", "Battery Consumed" };


    static string expenseReportFileName = "ExpenseReport.csv";
    static string[] budgetReportHeaders = new string[] { "Equipment", "Energy Consumption", "Maintenance", "Personnel", "Misc", "Initial Investment", "Daily", "Monthly", "Annually" };


    #region Actions

    public static void AppendToExpenseReport(string[] strings)
    {
        GetExpenseReportDirectoryPath();
        VerifyExpenseReportFile();
        using (StreamWriter sw = File.AppendText(GetExpenseReportFilePath()))
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

    public static string[] GetExpenseReportEntries()
    {
        VerifyExpenseReportDirectory();
        VerifyExpenseReportFile();
        return File.ReadAllLines(GetExpenseReportDirectoryPath());
    }

    public static void CreateReport()
    {
        using (StreamWriter sw = File.CreateText(GetExpenseReportFilePath()))
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

    public static void CreateExpenseReport()
    {
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string finalString = "";
            for (int i = 0; i < budgetReportHeaders.Length; i++)
            {
                if (finalString != "")
                    finalString += reportSeperator;

                finalString += budgetReportHeaders[i];
            }
            sw.WriteLine(finalString);
        }
    }

    public static void PrintReportPath()
    {
        Debug.Log("<b><color=orange>Report file found at:</color></b> " + GetDirectoryPath());
    }

    public static void PrintExpenseReportPath()
    {
        Debug.Log("<b><color=orange>Report file found at:</color></b> " + GetExpenseReportDirectoryPath());
    }

    #endregion

    #region Queries

    static void VerifyDirectory()
    {
        string dir = GetDirectoryPath();
        if (!Directory.Exists(GetDirectoryPath()))
            Directory.CreateDirectory(dir);
    }

    static void VerifyExpenseReportDirectory()
    {
        string dir = GetExpenseReportDirectoryPath();
        if (!Directory.Exists(GetExpenseReportDirectoryPath()))
            Directory.CreateDirectory(dir);
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
            CreateReport();
    }

    static void VerifyExpenseReportFile()
    {
        string file = GetExpenseReportFilePath();
        if (!File.Exists(file))
            CreateExpenseReport();
    }

    static string GetDirectoryPath()
    {
        return Application.persistentDataPath + "/" + reportDirectoryName;
    }


    static string GetExpenseReportDirectoryPath()
    {
        return Application.persistentDataPath + "/" + reportDirectoryName;
    }

    static string GetExpenseReportFilePath()
    {
        return GetDirectoryPath() + "/" + expenseReportFileName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    #endregion
}
