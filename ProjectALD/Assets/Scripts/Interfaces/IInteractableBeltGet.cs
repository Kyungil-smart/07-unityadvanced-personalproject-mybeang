using UnityEngine;

public interface IInteractableBeltGet
{
    public void InteractBeltGet<T>(T belt) where T : ObjectOnTile, IBelt;
}
