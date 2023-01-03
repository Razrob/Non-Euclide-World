using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Round(this Vector2 vector, float step)
    {
        return new Vector2(vector.x.Round(step), vector.y.Round(step));
    }

    public static Vector3 Round(this Vector3 vector, float step)
    {
        return new Vector3(vector.x.Round(step), vector.y.Round(step), vector.z.Round(step));
    }

    public static Vector2Int Round(this Vector2Int vector, int step)
    {
        return new Vector2Int(vector.x.Round(step), vector.y.Round(step));
    }

    public static Vector3Int Round(this Vector3Int vector, int step)
    {
        return new Vector3Int(vector.x.Round(step), vector.y.Round(step), vector.z.Round(step));
    }

    public static Vector3Int ToInt(this Vector3 vector)
    {
        return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    public static Vector2Int ToInt(this Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }

    public static Vector3 XY(this Vector3 vector) => new Vector3(vector.x, vector.y, 0f);
    public static Vector3 XZ(this Vector3 vector) => new Vector3(vector.x, 0f, vector.z);
    public static Vector3 YZ(this Vector3 vector) => new Vector3(0f, vector.y, vector.z);
    public static Vector3 X(this Vector3 vector) => new Vector3(vector.x, 0f, 0f);
    public static Vector3 Y(this Vector3 vector) => new Vector3(0f, vector.y, 0f);
    public static Vector3 Z(this Vector3 vector) => new Vector3(0f, 0f, vector.z);

    public static Vector3 SetX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);
    public static Vector3 SetY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
    public static Vector3 SetZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);
}
