using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class SaveFile
    {
        public bool completeTutorial;

        public SaveFile()
        {
            completeTutorial = GameManager.Instance.tutorialComplete;


        }
    }
}
