using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncVR.GeoGuesser
{
    [System.Serializable]
    public class GeoGuesserQuestion
    {
        public string correctAnswer;
        public List<string> wrongAnswers;
        public bool correct;

        public GeoGuesserQuestion ()
        {
            wrongAnswers = new List<string>();
        }

        public Texture GetQuestionTexture ()
        {
            return GeoGuesserService.Instance.GetLocation(correctAnswer).texture;
        }

        public List<string> ShuffledOptions ()
        {
            List<string> options = new List<string>();
            options.Add(correctAnswer);
            wrongAnswers.ForEach(x => options.Add(x));
            options.Shuffle();
            return options;
        }
    }

}
