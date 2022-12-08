using UnityEngine;

public static class Extensions
{
    public static int GetIndex(this LayerMask mask)
    {
        return (int)Mathf.Log(mask.value, 2);
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
