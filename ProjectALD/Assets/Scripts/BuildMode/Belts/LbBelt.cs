using System.Collections;
using UnityEngine;

public class LbBelt : ObjectOnTile, IInteractableBeltPut, IBeltBehavior
{
    public void InteractBeltPut(BaseBelt baseBelt)
    {
     
    }

    public IEnumerator GetItem()
    {
        yield return null;
    }

    public IEnumerator PutItem()
    {
        yield return null;
    }

    protected override void InitNumberOfConnectPoint()
    {
        
    }

    public override void PutOnTileHandler()
    {
        
    }
}
