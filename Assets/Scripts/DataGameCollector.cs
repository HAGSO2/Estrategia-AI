using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGameCollector : MonoBehaviour
{
    Dictionary<string, string> temporalFileDictionary = new Dictionary<string, string>();
    List<string> idsList = new List<string>();

    string m_Path;


    string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "GameData");
    }

    void OnApplicationQuit()
    {
        GetFinalRecord();
    }

    //When finish the game use the temporal records to create a final data sheet with the match
    public void GetFinalRecord()
    {

        //Obatins lector for both temporal files
        StreamReader sr1 = new StreamReader(temporalFileDictionary[idsList[0]]);
        StreamReader sr2 = new StreamReader(temporalFileDictionary[idsList[1]]);

        bool eof1 = false;
        bool eof2 = false;

        //Creates the final record based on a template
        var hash = new Hash128();
        hash.Append(System.DateTime.Now.ToString());
        string finarRecordPath = Path.Combine(filePath, hash.ToString() + ".csv");
        TextWriter tw = new StreamWriter(finarRecordPath, false);

        //Creates the table with the data
        tw.WriteLine(".;Archers;Barbarian;Giant;Goblin;Knight;Mini-Pekka");
        tw.WriteLine("=$A$4;\"=CONTAR.SI(B4:B20004;\"\"\"*Archer*\");\"=CONTAR.SI(B4:B20004;\"\"\"*Barbarian*\");\"=CONTAR.SI(B4:B20004;\"\"\"*Giant*\");\"=CONTAR.SI(B4:B20004;\"\"\"*Goblin*\");\"=CONTAR.SI(B4:B20004;\"\"\"*Knight*\");\"=CONTAR.SI(B4:B20004;\"\"\"*Mini-Pekka*\")");
        tw.WriteLine("=$C$4;\"=CONTAR.SI(D4:D20004;\"\"\"*Archer*\");\"=CONTAR.SI(D4:D20004;\"\"\"*Barbarian*\");\"=CONTAR.SI(D4:D20004;\"\"\"*Giant*\");\"=CONTAR.SI(D4:D20004;\"\"\"*Goblin*\");\"=CONTAR.SI(D4:D20004;\"\"\"*Knight*\");\"=CONTAR.SI(D4:D20004;\"\"\"*Mini-Pekka*\")");

        //Write the data of the temporal files
        while (!eof1 && !eof2)
        {
            string new_line = "";

            if (!eof1)
            {
                string data_string = sr1.ReadLine();
                if (data_string == null)
                {
                    eof1 = true;
                }
                new_line += data_string + ";";
            }

            if (!eof2)
            {
                string data_string = sr2.ReadLine();
                if (data_string == null)
                {
                    eof2 = true;
                }
                new_line += data_string + ";";
            }

            tw.WriteLine(new_line);
        }
        tw.Close();
        sr1.Close();
        sr2.Close();

        //Delete temporal files
        foreach (KeyValuePair<string, string> tempFile in temporalFileDictionary)
        {
            File.Delete(tempFile.Value);
            File.Delete(tempFile.Value + ".meta");
        }

        //Refresh the content browser
        UnityEditor.AssetDatabase.Refresh();
    }


    //Register a new action in the data collector made by an AI, if there where no previous records then creates a temporal file
    public void RegisterNewEntryData(string aiId, string name, string data)
    {
        if (!temporalFileDictionary.ContainsKey(aiId))
        {
            var hash = new Hash128();
            hash.Append(aiId + name);
            string tempFilePath = Path.Combine(filePath, hash.ToString() + ".csv");
            temporalFileDictionary.Add(aiId, tempFilePath);
            idsList.Add(aiId);
            TextWriter tw = new StreamWriter(tempFilePath, false);
            tw.WriteLine(name + ";" + data);
            tw.Close();
        }
        else
        {
            TextWriter tw = new StreamWriter(temporalFileDictionary[aiId], true);
            tw.WriteLine(name + ";" + data);
            tw.Close();
        }
        //Refresh the content browser
        UnityEditor.AssetDatabase.Refresh();
    }
}
