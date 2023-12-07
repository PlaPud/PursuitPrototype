using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string _fileDir = "";
    private string _fileName = "";

    public string FullPath => Path.Combine(_fileDir, _fileName);

    public FileDataHandler(string fileDir, string fileName)
    {
        _fileDir = fileDir;
        _fileName = fileName;
    }

    public GameData GetLoadFile() 
    {
        GameData loadedData = null;

        if (File.Exists(FullPath)) 
        {
            try
            {
                string data = "";

                using (FileStream fileStream = new (FullPath, mode: FileMode.Open)) 
                {
                    using (StreamReader streamReader = new (fileStream)) 
                    {
                        data = streamReader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(data);
            }
            catch (Exception err)
            {
                Debug.LogError(
                    "File Loading ERROR to Path " + FullPath + "\nError : " + err.Message
                );
            }
        }

        return loadedData;
    }

    public void SaveToFile(GameData data) 
    {
        try
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(FullPath)
            );

            string dataToStore = JsonUtility.ToJson(data, prettyPrint: true);

            using (FileStream fileStream = new (FullPath, mode: FileMode.Create)) 
            {
                using (StreamWriter streamWriter = new (fileStream)) 
                {
                    streamWriter.WriteAsync(dataToStore);
                }
            }
        }
        catch (Exception err) 
        {
            Debug.LogError(
                "File Saving ERROR to Path " + FullPath + "\nError : " + err.Message
            );
        }
    }
}
