using UnityEngine;

public static class Extensions
{
    public static int GetIndex(this LayerMask mask)
    {
        return (int)Mathf.Log(mask.value, 2);
    }
}
