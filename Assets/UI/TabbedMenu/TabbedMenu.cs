// This script attaches the tabbed menu logic to the game.
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;


//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class TabbedMenu : MonoBehaviour
{
    private TabbedMenuController controller;
    public GameObject volume;
    public GameObject sun;

    public enum BooleanFields
    {
        RandomizedSun,
        PreventSunOcclusion

    }

    public enum TextFields { 
        Dataset_Path
    }
    private void OnEnable()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;
        
        controller = new TabbedMenuController(root,volume.GetComponent<Volume>(),sun);

        controller.RegisterTabCallbacks();
    }



    public void UpdateImageName(string str) {
        controller.UpdateImageName(str);
    }

    public bool GetBooleanField(BooleanFields field) {
        if (field == BooleanFields.RandomizedSun)
            return controller.randomizeSun.value;
        else if (field == BooleanFields.PreventSunOcclusion)
            return controller.preventSunOcclusion.value;
        else 
            return false;
    }

    public string GetTextField(TextFields field)
    {

        if (field == TextFields.Dataset_Path)
            return controller.datasetPath.value;
        else
            return "None";
    }
}