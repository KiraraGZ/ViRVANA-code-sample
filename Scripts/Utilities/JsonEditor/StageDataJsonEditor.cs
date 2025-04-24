using System.IO;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Utilities.Json.Editor
{
    public class StageDataJsonEditor : MonoBehaviour
    {
        public string fileName = "GameDatas/StageDatas";
        public int StageIndex;
        public StageData EditStageData;
        public StageCollection StageCollection;

        private const string filePath = "/Resources/";

        [ContextMenu("Get Stage Data")]
        public void GetStageData()
        {
            if (StageIndex >= StageCollection.Count)
            {
                EditStageData = new();
                return;
            }

            // EditStageData = new(StageCollection.StageDatas[StageIndex]);
        }

        [ContextMenu("Set Stage Data")]
        public void SaveStageData()
        {
            if (StageIndex >= StageCollection.Count)
            {
                // StageCollection.StageDatas.Add(new(EditStageData));
                return;
            }

            // StageCollection.StageDatas[StageIndex] = new(EditStageData);
            OverwriteJsonFile();
        }

        // public void InsertStageData()
        // {
        //     EditStageData = new();
        //     StageCollection.StageDatas.Insert(++StageIndex, EditStageData);
        //     GetStageData();
        //     OverwriteJsonFile();
        // }

        // public void RemoveStageData()
        // {
        //     StageCollection.StageDatas.RemoveAt(StageIndex);

        //     if (StageIndex >= StageCollection.StageDatas.Count)
        //     {
        //         StageIndex = StageCollection.StageDatas.Count - 1;
        //         GetStageData();
        //     }

        //     OverwriteJsonFile();
        // }

        public void SwapStageData(int first, int second)
        {
            (StageCollection.StageDatas[second], StageCollection.StageDatas[first]) = (StageCollection.StageDatas[first], StageCollection.StageDatas[second]);
        }

        [ContextMenu("Write JSON file")]
        public void WriteJsonFile()
        {
            string jsonText = JsonUtility.ToJson(StageCollection);

            Debug.Log(jsonText);

            StreamWriter outStream = File.CreateText($"{Application.dataPath + filePath}{fileName}.json");
            outStream.Write(jsonText);
            outStream.Flush();
            outStream.Close();
        }

        [ContextMenu("Overwrite JSON file")]
        public void OverwriteJsonFile()
        {
            string jsonText = JsonUtility.ToJson(StageCollection);

            Debug.Log($"Overwrite StageCollection : {StageCollection.Count}");

            StreamWriter outStream = new($"{Application.dataPath + filePath}{fileName}.json", false);
            outStream.Write(jsonText);
            outStream.Flush();
            outStream.Close();
        }

        [ContextMenu("Read JSON file")]
        public void ReadJsonFile()
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>(fileName);
            StageCollection = JsonUtility.FromJson<StageCollection>(jsonTextFile.text);

            Debug.Log($"Read StageCollection : {StageCollection.Count}");
        }
    }
}
