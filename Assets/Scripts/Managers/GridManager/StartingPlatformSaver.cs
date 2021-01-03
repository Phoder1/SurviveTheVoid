using UnityEngine;
using System;
using System.IO;

public class StartingPlatformSaver : MonoBehaviour
{
    object WriteReadLock = new object();
    private string buildBasePath;
    private string DirectoryPath { get => buildBasePath + "/Saves/"; }
    StartingPlatform platform;
    //public enum SaveFile { Test, HomeworkData };

    private void Start() {
        platform = new StartingPlatform();
        buildBasePath = Application.dataPath;
    }
    private string GetFilePath() { return DirectoryPath + "platform" + ".txt"; }
    internal string GetJson() { return JsonUtility.ToJson(platform, true); }
    public bool FileExists() { return File.Exists(GetFilePath()); }
    public void SavePlatform() {
        string data = GetJson();
        string filePath = GetFilePath();
        if (Directory.Exists(DirectoryPath)) {
            lock (WriteReadLock) {
                File.WriteAllText(filePath, data);
            }
        }
        else {
            lock (WriteReadLock) {
                Directory.CreateDirectory(DirectoryPath);
            }
            SavePlatform();
        }


    }

    public void LoadPlatform() {
        string filePath = GetFilePath();
        if (FileExists()) {
            lock (WriteReadLock) {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), platform);
            }
        }
    }



    [Serializable]
    public class StartingPlatform
    {
        public TileSlot[,] floorTiles;
        public TileSlot[,] buildingsTiles;
        public Vector2Int startPos;
    }

}
