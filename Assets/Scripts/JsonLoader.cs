using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadsheetSystems
{
    public class JsonLoader : MonoBehaviour
    {
        // URLは環境に応じて変更
        string filePath = "assets/Resources/question.json";
        [SerializeField]bool _isPrintQuestion = false;
        [SerializeField]int _questionId = 1;

        private void Update()
        {
            if (_isPrintQuestion) JsonWrite();
        }


        void JsonWrite()
        {
            string jsonText = Resources.Load<TextAsset>("question").ToString();

            JsonNode json = JsonNode.Parse(jsonText);
            //Debug.Log(jsonText);
            Debug.Log("requestID = " + _questionId);
            foreach (var note in json["questions"])
            {
                int id = int.Parse(note["id"].Get<string>());
                string question = note["question"].Get<string>();
                string answer = note["answer"].Get<string>();
                //簡易的な指定問題表示方法（効率悪め）

                if (id == _questionId)
                {
                    Debug.Log(question);
                    Debug.Log(answer);
                }
            }
            _isPrintQuestion = false;
        }


    }
}