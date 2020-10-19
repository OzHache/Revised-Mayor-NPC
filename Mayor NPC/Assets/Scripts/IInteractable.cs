using System;
using System.Collections;
using System.Collections.Generic;

public interface IInteractable
{
    /* Returns an array of strings that are used to identify the object and it's interactions
     * The last item is always the Description
     */
    
    Dictionary<UIButtonValues,string> Identify();
   
}
public enum InteractionTypes
{
    Take,
    Use,
    Misc,
    Unused,
    Build, 
    Add
}
