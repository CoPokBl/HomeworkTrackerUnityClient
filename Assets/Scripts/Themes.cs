using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Themes : MonoBehaviour {
    
    public static readonly Theme[] PresetThemes = {
        new Theme {
            Name = "Light",
            BackgroundColor = Color.white,
            ForegroundColor = Color.white,
            TextColour = Color.black,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "Gray",
            BackgroundColor = Color.gray,
            ForegroundColor = Color.gray,
            TextColour = Color.white,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "Black",
            BackgroundColor = Color.black,
            ForegroundColor = Color.gray,
            TextColour = Color.white,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "Blue",
            BackgroundColor = Color.cyan,
            ForegroundColor = Color.blue,
            TextColour = Color.red,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "Green",
            BackgroundColor = Color.gray,
            ForegroundColor = Color.green,
            TextColour = Color.cyan,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "Red",
            BackgroundColor = Color.gray,
            ForegroundColor = Color.red,
            TextColour = Color.cyan,
            ErrorColour = Color.red
        },
        new Theme {
            Name = "RGB",
            BackgroundColor = Color.red,
            ForegroundColor = Color.green,
            TextColour = Color.blue,
            ErrorColour = Color.white
        }
    };
    
    public static Theme CurrentTheme;
    
    private readonly List<Transform> _children = new List<Transform>();


    private void GetChildren(Transform trans) {
        // get all children of trans
        for (int i = 0; i < trans.childCount; i++) {
            Transform child = trans.GetChild(i);
            _children.Add(child);
            GetChildren(child);
        }
    }

    private void Start() {
        FileLogging.Debug("Loading theme for scene");
        int theme = PlayerPrefs.GetInt("theme", 0);
        CurrentTheme = PresetThemes[theme];
        SetTheme();
    }

    public void SetTheme() => ApplyThemeTo(transform);

    public void ApplyThemeTo(Transform obj) {
        try {
            FileLogging.Debug($"Applying theme to {obj.name}");
            // Get theme from settings
            int theme = PlayerPrefs.GetInt("theme", 0);
            CurrentTheme = PresetThemes[theme];
        
            // Get all children
            GetChildren(obj);

            foreach (Transform child in _children) {
                foreach (Component component in child.GetComponents<Component>()) {
                    switch (component) {
                        case Image image when child.name == "Checkmark":
                            image.color = PresetThemes[theme].TextColour;
                            break;
                        case Image image when child.name == "background":
                            // background
                            image.color = PresetThemes[theme].BackgroundColor;
                            continue;
                        case InputField inputField:
                            inputField.image.color = PresetThemes[theme].ForegroundColor;
                            continue;
                        case Text text:
                            if (text.color == Color.red) {
                                text.color = PresetThemes[theme].ErrorColour;
                                continue;
                            }
                            text.color = PresetThemes[theme].TextColour;
                            break;
                        case Button button:
                            button.image.color = PresetThemes[theme].ForegroundColor;
                            break;
                        case Toggle toggle:
                            toggle.targetGraphic.color = PresetThemes[theme].ForegroundColor;
                            break;
                        case Dropdown dropdown:
                            child.GetComponent<Image>().color = PresetThemes[theme].ForegroundColor;
                            break;
                    }
                }
            }
        
            _children.Clear();
            FileLogging.Debug("Applied theme to all children");
        }
        catch (Exception e) {
            FileLogging.Error("Failed to apply theme");
            FileLogging.Error(e.ToString());
        }
    }
    
    
}