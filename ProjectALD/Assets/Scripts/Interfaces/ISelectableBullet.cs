using System.Collections.Generic;
using UnityEngine;

public interface ISelectableBullet
{
    public void ClearStorage();
    public void SetStorage();
    public void SelectBullet(ItemType itemType);
}
