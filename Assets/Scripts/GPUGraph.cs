using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;

    [SerializeField, Range(10,maxResolution)] int resolution = 10;

    public event Action<FunctionLibrary.FunctionName> OnFunctionChanged;

    private FunctionLibrary.FunctionName _function;

    [SerializeField] public FunctionLibrary.FunctionName function
    {
        get { return _function; }
        set
        {
            _function = value;
            OnFunctionChanged?.Invoke(_function); // Notify subscribers when the function changes
        }
    }

    [SerializeField] ComputeShader computeShader;

    [SerializeField] Material material;

    [SerializeField] Mesh mesh;

    public enum TransitionMode { Cycle, Random }

    [SerializeField] TransitionMode transitionMode;

    [SerializeField, Min(1f)] float functionDuration = 3f, transitionDuration = 1f;

    float duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;

    static readonly int positionsId = 
        Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    private void OnEnable ()
    {
        // Compute Buffer parameters - # of arbitrary elements, size per element
        // size of each element is 3*4 as each element consists of:
        //  - 3D Position Vectors (which is three float numbers)
        //  - one vector is 4 bytes
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable()
    {
        // frees the buffer
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning) 
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }     

        UpdateFunctionOnGPU();
    }

    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        var kernelIndex =
            (int)function +
            (int)(transitioning ? transitionFunction : function) *
            FunctionLibrary.FunctionCount;

        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }


    // getters for UIManager
    public void setResolution(int res)
    {
        resolution = res;
    }

    public void setTransitionMode(int index)
    {
        transitionMode = index == 0 ? TransitionMode.Cycle : TransitionMode.Random;
    }

    public void setFuncDuration(int dur)
    {
        functionDuration = dur;
    }

    public void setTransDuration(int dur)
    {
        transitionDuration = dur;
    }
}
