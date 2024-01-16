using AlgernonCommons.Notifications;
using System;
using System.IO;

namespace POAIDBOX.Structure
{
    public class Mechanism
    {
        public static void Main()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string colossalOrderFolder = "Colossal Order";
            string citiesSkylinesFolder = "Cities_Skylines";
            string proceduralObjectsSettingsFile = "ProceduralObjectsSettings.cgs";
            string proceduralObjectsKeyBindingsFile = "ProceduralObjects_KeyBindings.cfg";

            string colossalOrderPath = Path.Combine(localAppData, colossalOrderFolder);
            string citiesSkylinesPath = Path.Combine(colossalOrderPath, citiesSkylinesFolder);
            string proceduralObjectsSettingsPath = Path.Combine(citiesSkylinesPath, proceduralObjectsSettingsFile);
            string proceduralObjectsKeyBindingsPath = Path.Combine(citiesSkylinesPath, proceduralObjectsKeyBindingsFile);

            try
            {
                // Delete ProceduralObjectsSettings.cgs if it exists
                if (File.Exists(proceduralObjectsSettingsPath))
                {
                    File.Delete(proceduralObjectsSettingsPath);
                    UnityEngine.Debug.Log("Deleted ProceduralObjectsSettings.cgs");
                }

                // Delete ProceduralObjects_KeyBindings.cfg if it exists
                if (File.Exists(proceduralObjectsKeyBindingsPath))
                {
                    File.Delete(proceduralObjectsKeyBindingsPath);
                    UnityEngine.Debug.Log("Deleted ProceduralObjects_KeyBindings.cfg");
                }

                CompletedNotification notification = NotificationBase.ShowNotification<CompletedNotification>();
                notification.AddParas("Operation completed. Perform the following steps:"
                    + "\n1. Restart the game and proceed to the main menu."
                    + "\n2. Disable the Procedural Objects (PO) mod, then re-enable it."
                    + "\n3. Load into your city."
                    + "\n4. Completely close the game.");

            }
            catch (Exception ex)
            {
                CompletedNotification notification = NotificationBase.ShowNotification<CompletedNotification>();
                notification.AddParas("Operation failed. Ensure Cities: Skylines has admin permissions or the files were not found.");
            }

            
        }
    }
}
