using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    public enum FunctionName { Wave, MultiWave, Ripple, Circling, Cylindering, MobiusStrip, Sphere, HourGlass, VariedSphere, TwistedSphere, Torus}

    static Function[] functions = { Wave, MultiWave, Ripple, Circling, Cylindering, MobiusStrip, Sphere, HourGlass, VariedSphere, TwistedSphere, Torus};

    public static Function GetFunction (FunctionName name)
    {
        return functions[(int)name];
    }

    // returns a sine wave
    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u * 0.8f;
        p.y = Sin(PI * (u + v + t)) * 0.8f;
        p.z = v * 0.8f;
        return p;
    }

    public static Vector3 MultiWave (float u, float v, float t)
    {
        // adds a new sine wave with double the frequency
        // doubling freq = muliplying the sine function by 2
        // and dividing it by half to retain the shape of the
        // old sine wave.
        // slowed sine wave slowed to a quarter (0.25f)

        // multiwave = old sine wave + double freq sine wave + slowed sine wave
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        // float d is distance
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }
    public static Vector3 Circling(float u, float v, float t)
    {
        Vector3 p;
        p.x = Sin(PI * u + (t * 0.5f));
        p.y = 0f;
        p.z = Cos(PI * u + (t * 0.5f));
        return p;
    }

    public static Vector3 Cylindering(float u, float v, float t)
    {
        Vector3 p;
        p.x = Sin(PI * u + (t * 0.5f));
        p.y = v + Sin(PI * u + (t * 0.5f)) * Sin(PI * u + (t * 0.5f));
        p.y += Sin(PI * (u + t * 0.5f));
        p.y *= 0.5f;
        p.z = Cos(PI * u + (t * 0.5f));
        return p;
    }

    public static Vector3 HourGlass(float u, float v, float t)
    {
        // float r is radius
        float r = Cos(0.5f * PI * v + (t * 0.5f));
        Vector3 p;
        p.x = r * Sin(PI * u + (t * 0.2f));
        p.y = v;
        p.z = r * Cos(PI * u + (t * 0.2f));
        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.5f + 0.5f * Sin(PI * t * 0.5f);
        // float s manipulates surface of the sphere
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 VariedSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Sin(8f * PI * u);
        float s = r * Cos(0.3f * PI * (3f * u + 2f * v + t));
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 TwistedSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(PI * 0.5f * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Torus (float u, float v, float t)
    {
        // float r = 1f;
        float outRad = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float inRad = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = outRad + inRad * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = inRad * Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;

    }

    public static Vector3 MobiusStrip(float u, float v, float t)
    {
        Vector3 p;
        float stripWidth = 0.2f;
        float r = 0.5f;
        p.x = (r + stripWidth * v * Cos(0.5f * PI * u + (0.5f * t))) * Cos(PI * u + (0.15f * t));
        p.y = (r + stripWidth * v * Cos(0.5f * PI * u + (0.5f * t))) * Sin(PI * u + (0.15f * t));
        p.z = stripWidth * v * Sin(0.5f * PI * u + (0.5f * t));

        // scale
        p.x *= 1.5f;
        p.y *= 1.5f;
        p.z *= 1.5f;
        return p;

    }

    // morphs the coordinates u v t from the previous function, to the new function by linear interpolation
    // used smoothstep to make it more fluid
    public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }

    public static FunctionName GetNextFunctionName(FunctionName name)
    {
        return (int)name < functions.Length - 1 ? name + 1 : 0;
    }

    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
    {
        var choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0 : choice;
    }

    public static int FunctionCount => functions.Length;

    
}
