using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Utility
{
    public class SaveFile
    {
        public bool completeTutorial;

        //default save file
        public SaveFile() 
        {
            completeTutorial = false;
        }

        //save file
        public SaveFile(CoreManager core)
        {
            completeTutorial = core.completeTutorial;
        }
    }

    public class SaveManager
    {
#if UNITY_EDITOR
        public static string savePath = Application.persistentDataPath + "/saveFile.json";

#else
        public static string savePath = Application.persistentDataPath + "/saveFile.json";

#endif
        public static SaveFile Load()
        {
            string saveString = GetSaveString();

            if (saveString != string.Empty)
            {
                try
                {
                    return JsonConvert.DeserializeObject<SaveFile>(saveString);
                }
                catch
                {
                    SaveFile save = new SaveFile();
                    Save(save); //overwrite corrupted file
                    return save;
                }
            }
            else
            {
                SaveFile save = new SaveFile();
                Save(save); //overwrite corrupted file
                return save;
            }
        }

        public static string GetSaveString()
        {
            //eventually add decode in here
            if (File.Exists(savePath))
                return File.ReadAllText(savePath);
            else return string.Empty;
        }

        public static void Save(SaveFile save)
        {
            Dictionary<string, object> saveParameters = new Dictionary<string, object>();

            saveParameters.Add("completeTutorial", save.completeTutorial);

            string data = JsonConvert.SerializeObject(saveParameters, Formatting.Indented);

            File.WriteAllText(savePath, data);
        }
    }
}
