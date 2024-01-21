using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string _fileDir = "";
    private string _fileName = "";
    private bool _useEncryption = false;

    private readonly string _encryptionKey = "INTENSITY";
    private readonly string _backupExtension = ".bak";

    public string FullPath => Path.Combine(_fileDir, _fileName);
    public string BackupPath => Path.Combine(_fileDir, _fileName + _backupExtension);

    public FileDataHandler(string fileDir, string fileName, bool useEncryption)
    {
        _fileDir = fileDir;
        _fileName = fileName;
        _useEncryption = useEncryption;
    }

    public GameData LoadFromFile(bool allowRestoreFromBackup = true) 
    {
        GameData loadedData = null;
        Debug.Log("Load File From: " + FullPath);

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

                if (_useEncryption) data = EncryptDecrypt(data);

                loadedData = JsonConvert.DeserializeObject<GameData>(data); 
            }
            catch (Exception err)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to Load Saved Data. Attempting to roll back.\n" + err);
                    bool isSuccess = AttemptRollback();
                    if (isSuccess)
                    {
                        loadedData = LoadFromFile(allowRestoreFromBackup: false);
                    }
                }
                else 
                {
                    Debug.LogError("Failed to Load Saved Data From Backup File.\n" + err);
                }
            }
        }

        return loadedData;
    }

    public void SaveToFile(GameData data) 
    {
        if (GameManager.Instance) GameManager.Instance.IsSaving = true;

        try
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(FullPath)
            );
            string dataToStore = JsonConvert.SerializeObject(data, formatting: Formatting.None);

            if (_useEncryption) dataToStore = EncryptDecrypt(dataToStore);

            using (FileStream fileStream = new (FullPath, mode: FileMode.Create)) 
            {
                using (StreamWriter streamWriter = new (fileStream)) 
                {
                    streamWriter.Write(dataToStore);
                }
            }

            GameData verifiedGameData = LoadFromFile();

            if (verifiedGameData != null) 
            {
                File.Copy(FullPath, BackupPath, overwrite: true);
            }
            else
            {
                throw new Exception("Data Verification Failed. Can't Create Backup.");
            }
        }
        catch (Exception err) 
        {
            Debug.LogError(
                "File Saving ERROR to Path " + FullPath + "\nError : " + err.Message
            );
        }

        if (GameManager.Instance) GameManager.Instance.IsSaving = false;
    }

    private string EncryptDecrypt(string data)
    {
        string cypher = "";

        for (int i = 0; i < data.Length; i++)
        {
            cypher += (char) (data[i] ^ _encryptionKey[i % _encryptionKey.Length]);
        }

        return cypher;
    }

    private bool AttemptRollback()
    {
        bool isSuccess = false;
        try
        {
            if (File.Exists(BackupPath))
            {
                File.Copy(BackupPath, FullPath, overwrite: true);
                isSuccess = true;
                Debug.LogWarning("File Rollback to Path " + BackupPath);
            }
            else 
            {
                throw new Exception("Backup File Not Found.");
            }
        }
        catch (Exception err) 
        {
            Debug.LogError(
                "File Rollback ERROR to Path " + BackupPath + "\nError : " + err.Message
            );
        }
        return isSuccess;
    }
}
