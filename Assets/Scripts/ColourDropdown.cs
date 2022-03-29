using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Color = API.Color;

public class ColourDropdown : MonoBehaviour {
    
    private static string[] _colours =
    {
        "Default",
        "Red", 
        "Green", 
        "Blue",
        "Violet", 
        "Pink", 
        "Black", 
        "White"
    };

    private static Color[] _colourValues;
    
    private void Start() {
        Dropdown me = GetComponent<Dropdown>();
        
        me.options.Clear();
        // add the colours to the dropdown
        foreach (string colour in _colours) {
            me.options.Add(new Dropdown.OptionData(colour));
        }

        if (_colourValues == null) {
            InitColourValues();
        }
    }

    private static void InitColourValues() {
        // fill the colour values array with the correct colours
        _colourValues = new Color[_colours.Length];
        for (int i = 0; i < _colours.Length; i++) {
            _colourValues[i] = new Color(_colours[i]);
        }
    }
    
    // function to get the name of the selected colour
    public static string GetSelectedColour(Color c) {
        if (_colourValues == null) {
            InitColourValues();
        }
        
        for (int i = 0; i < _colourValues.Length; i++) {
            if (c == _colourValues[i]) {
                return _colours[i];
            }
        }
        return "Default";
    }

    // create function that returns the colour of a number
    public static string GetColour(int colourNumber) {
        return _colours[colourNumber];
    }
    
    // create function that returns the number of a colour
    public static int GetColourNumber(string colour) {
        for (int i = 0; i < _colours.Length; i++) {
            if (_colours[i] == colour) {
                return i;
            }
        }
        return -1;
    }
    
}
