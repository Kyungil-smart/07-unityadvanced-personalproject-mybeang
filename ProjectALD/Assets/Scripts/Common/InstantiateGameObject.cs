using UnityEngine;

public static class BuildingObject
{
    public static GameObject Create(GameObject prefab, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
    {
        int objectId = DataManager.Instance.GetObjectId();
        GameObject gObject = Object.Instantiate(prefab, position, rotation);
        gObject.name = $"{gObject.name}-{objectId}";
        return gObject;
    } 
}
