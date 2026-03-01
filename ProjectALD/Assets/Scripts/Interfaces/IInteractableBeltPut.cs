using UnityEngine;

public interface IInteractableBeltPut
{
    public void InteractBeltPut<T>(T belt) where T : ObjectOnTile, IBelt;
}
