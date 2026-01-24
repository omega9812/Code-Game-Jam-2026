using UnityEngine;
using UnityEditor;

public class ExecuteColliderConversion
{
    public static void Execute()
    {
        ConvertCollidersExecutor.ConvertAllCollidersToCircle();
    }
}