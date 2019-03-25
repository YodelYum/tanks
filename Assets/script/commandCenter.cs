using CommandTerminal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commandCenter : MonoBehaviour
{
    [RegisterCommand(Help = "Set TopSpeed of Tank", MinArgCount = 1, MaxArgCount = 1)]
    static void topspeed(CommandArg[] args)
    {
        GameObject.Find("MagicSteeringTank").GetComponent<PhysicsTank>().topSpeed = args[0].Int;
        Debug.Log("TopSpeed set to: "+ args[0].ToString());
    }

    [RegisterCommand(Help = "Show/Hide mouse", MinArgCount = 1, MaxArgCount = 1)]
    static void mouse(CommandArg[] args)
    {
        if(args[0].String == "hide")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Mouse is now hidden.");
        }
        else if (args[0].String == "show")
        {
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Mouse is now visible.");
        }

        
    }
}
