using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

using static FunctionLibrary;

public class UIManager : MonoBehaviour
{
    [SerializeField] GPUGraph graph;

    private VisualElement root;
    private SliderInt res;
    private DropdownField func;
    private RadioButtonGroup transitionMode;
    private IntegerField funcDuration;
    private IntegerField transDuration;

    private Dictionary<string, FunctionLibrary.FunctionName> functionMappings = new Dictionary<string, FunctionLibrary.FunctionName>();

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        res = root.Q<SliderInt>("Resolution");
        func = root.Q<DropdownField>("Function");
        transitionMode = root.Q<RadioButtonGroup>("TransitionMode");
        funcDuration = root.Q<IntegerField>("FunctionDuration");
        transDuration = root.Q<IntegerField>("TransitionDuration");

        RegisterValuesUI();
        
    }

    private void RegisterValuesUI()
    {
        if (res != null)
        {
            res.RegisterValueChangedCallback(evt => UpdateResolution());
        }
        else
        {
            Debug.LogError("Resolution not found in root.");
        }

        if (func != null)
        {
            func.RegisterValueChangedCallback(evt => UpdateFunction());
            InitializeFunctionMappings();

            graph.OnFunctionChanged += OnFunctionChanged;
        }
        else
        {
            Debug.LogError("Function not found in root.");
        }

        if (transitionMode != null)
        {
            transitionMode.RegisterValueChangedCallback(evt => UpdateTransitionMode(evt.newValue)); ;
        }
        else
        {
            Debug.LogError("Transition Mode not found in root.");
        }

        if (funcDuration != null)
        {
            funcDuration.RegisterValueChangedCallback(evt => UpdateFunctionDuration());
        }
        else
        {
            Debug.LogError("Function Duration not found in root.");
        }

        if (transDuration != null)
        {
            transDuration.RegisterValueChangedCallback(evt => UpdateTransitionDuration());
        }
        else
        {
            Debug.LogError("Transition Duration not found in root.");
        }
    }

    private void UpdateResolution()
    {
        graph.setResolution(res.value);
    }

    private void UpdateFunction()
    {
        // Check if the selected value exists in the dictionary
        if (functionMappings.TryGetValue(func.value, out FunctionLibrary.FunctionName functionName))
        {
            // Update the function in the GPUGraph script
            graph.function = functionName;
        }
        else
        {
            Debug.LogError($"Function mapping not found for value: {func.value}");
        }
    }

    private void UpdateTransitionMode(int index)
    {
        graph.setTransitionMode(index);
    }

    private void UpdateFunctionDuration()
    {
        graph.setFuncDuration(funcDuration.value != 0 ? funcDuration.value : 1);

        if (funcDuration.value == 0) funcDuration.value = 1;
    }

    private void UpdateTransitionDuration()
    {
        graph.setTransDuration(transDuration.value != 0 ? transDuration.value : 1);

        if (transDuration.value == 0) transDuration.value = 1;
    }


    // Helper function for Function Names
    private void InitializeFunctionMappings()
    {
        // Add mappings based on your dropdown values and enum values
        functionMappings.Add("Wave", FunctionLibrary.FunctionName.Wave);
        functionMappings.Add("Multi Wave", FunctionLibrary.FunctionName.MultiWave);
        functionMappings.Add("Ripple", FunctionLibrary.FunctionName.Ripple);
        functionMappings.Add("Circling", FunctionLibrary.FunctionName.Circling);
        functionMappings.Add("Cylindering", FunctionLibrary.FunctionName.Cylindering);
        functionMappings.Add("Mobius Strip", FunctionLibrary.FunctionName.MobiusStrip);
        functionMappings.Add("Sphere", FunctionLibrary.FunctionName.Sphere);
        functionMappings.Add("Hour Glass", FunctionLibrary.FunctionName.HourGlass);
        functionMappings.Add("Varied Sphere", FunctionLibrary.FunctionName.VariedSphere);
        functionMappings.Add("Twisted Sphere", FunctionLibrary.FunctionName.TwistedSphere);
        functionMappings.Add("Torus", FunctionLibrary.FunctionName.Torus);
    }

    private string GetDropdownValueForFunction(FunctionLibrary.FunctionName functionName)
    {
        // what the hell? Adds space where there is capitalized letters in the middle
        return Regex.Replace(functionName.ToString(), "(?<=.)([A-Z])", " $1");
    }

    private void OnFunctionChanged(FunctionLibrary.FunctionName newFunction)
    {
        // Update the dropdown value when the function changes
        func.value = GetDropdownValueForFunction(newFunction);
    }
}
