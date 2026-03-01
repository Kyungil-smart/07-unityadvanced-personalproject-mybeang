using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMaster : ObjectOnTile, IMovableBuilding, IInteractableBeltGet, IInteractableBeltPut, ISelectableBullet
{
    // Inspector
    public ItemType curBulletType;
    [SerializeField] private GameObject _bulletBoxPrefab;
    [SerializeField] private List<GameObject> _bulletPrefabs;  // pairing to _cacheBullets
    [SerializeField] private GameObject _panel;
    [SerializeField] private SpriteRenderer _bulletSROnPanel;
    [SerializeField] private List<FactorySlave> _factorySlaves;
    
    // Storage
    private Dictionary<ItemType, Queue<Item>> InputStorage;
    private Queue<Item> _outputStorage;
    
    // Others
    private readonly int _maxInput = 10;
    private readonly int _maxOutput = 10;
    private BulletBox _bulletBox;
    private List<Item> _cacheBullets;  // pairing to bulletPrefabs
    private Coroutine _createCoroutine;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        _outputStorage = new();
        _cacheBullets = new();
        foreach (var prefab in _bulletPrefabs)
        {
            _cacheBullets.Add(prefab.GetComponent<Item>()); 
        }

        InitNumberOfConnectPoint();
    }

    private (Item item, GameObject prefab) GetBulletData(ItemType itemType)
    {
        int index = -1; 
        for (int i = 0; i < _cacheBullets.Count; i++)
        {
            if (_cacheBullets[i].itemType == itemType)
            {
                index = i;
                break;
            }
        }
        if (index == -1) return (null, null);
        return (_cacheBullets[index], _bulletPrefabs[index]);
    }
    
    protected override void InitNumberOfConnectPoint()
    {
        // ToDo. factory slave 가 있는 경우 고려 필요.
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.East, null));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.West, null));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.South, null));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.North, null));  
    }
    
    public override void PutOnTileHandler()
    {
        _spriteRenderer.sortingLayerName = "FactoryMine";
        // ToDo. factory slave 가 있는 경우 고려 필요.
        foreach (var both in bothes)
        {
            ConnectToNeighbor(both);
        }
        
        // Test 용. 추후 UI 로 select 진행.
        ReadyToCreate(ItemType.Arrow); 
        
        // connectable 확인 된 후에 진행.
        _createCoroutine = StartCoroutine(CreateCroutine());
    }

    private void ReadyToCreate(ItemType itemType)
    {
        curBulletType = itemType;
        InputStorage = new ();
        
        // ToDo. 레시피 참고해서 데이터 구성 필요
        InputStorage.Add(ItemType.Copper, new Queue<Item>());
        
        // 공장에 간판 걸기
        (var item, var prefab) = GetBulletData(curBulletType);
        _bulletSROnPanel.sprite = item.spriteRenderer.sprite;
        _panel.SetActive(true);
    }

    public override void TakeOffTileHandler()
    {
        if (_createCoroutine != null) StopCoroutine(_createCoroutine);
        _createCoroutine = null;
    }

    public void InteractBeltGet<T>(T belt) where T : ObjectOnTile, IBelt
    {
        if (_outputStorage.Count > 0 && belt.item == null)
        {
            Item bulletBox = _outputStorage.Dequeue();
            belt.item = bulletBox;
        }
    }

    private IEnumerator CreateCroutine()
    {
        while (true)
        {
            CreateBulletBox();
            yield return new WaitForSeconds(1f);
        }
    }

    private void ReadyBulletBox()
    {
        // ToDo. Object Pool 관리 필요.
        GameObject bulletBoxObj = Instantiate(_bulletBoxPrefab);
        bulletBoxObj.SetActive(false);
        _bulletBox = bulletBoxObj.GetComponent<BulletBox>();
        (Item bullet, GameObject bulletObj) = GetBulletData(curBulletType);
        _bulletBox.SetLabel(curBulletType, bullet.spriteRenderer);
    }
    
    private bool IsEnoughResources()
    {
        PrintLog("재고를 확인한다.");
        bool result = true;
        if (_bulletBox == null) return false;
        PrintLog("BulletBox 는 확인되었다.");
        if (_bulletBox.data.copperCnt > 0)
            result = InputStorage[ItemType.Copper].Count >= _bulletBox.data.copperCnt;
        if (_bulletBox.data.iconCnt > 0)
            result = InputStorage[ItemType.Iron].Count >= _bulletBox.data.iconCnt;
        if (_bulletBox.data.fireEleCnt > 0)
            result = InputStorage[ItemType.Fire].Count >= _bulletBox.data.fireEleCnt;
        if (_bulletBox.data.iceEleCnt > 0)
            result = InputStorage[ItemType.Ice].Count >= _bulletBox.data.iceEleCnt;
        PrintLog($"재고가 충분한가? {result}");
        if (!result)
        {
            PrintLog($"{ItemType.Copper} 재고: {InputStorage[ItemType.Copper].Count}");
        }
        return result;
    }

    private void WastResource(ItemType itemType, int count)
    {
        if (count > 0)
            for (int i = 0; i < count; i++)
                InputStorage[itemType].Dequeue();
    }
    
    private void CreateBulletBox()
    {
        // TODO: State pattern 으로 가능할 것 같음.
        PrintLog("탄환 박스 생성 시작.");
        // bulletbox 가 없으면 생성
        if (_bulletBox == null)
            ReadyBulletBox();
        PrintLog("탄환 박스가 준비되었다.");
        // 창고에 충분한 리소스들이 있는지 확인
        if (!IsEnoughResources()) return;
        
        // 소비 
        PrintLog("자원을 소비한다.");
        WastResource(ItemType.Copper, _bulletBox.data.copperCnt);
        WastResource(ItemType.Iron, _bulletBox.data.iconCnt);
        WastResource(ItemType.Fire, _bulletBox.data.fireEleCnt);
        WastResource(ItemType.Ice, _bulletBox.data.iceEleCnt);
        
        // 생산
        PrintLog("탄환을 생산해 탄환 박스에 넣는다.");
        (Item bullet, GameObject bulletPrefab) = GetBulletData(curBulletType);
        while (!_bulletBox.IsFull())
        {
            // ToDo. Object Pool 최적화 필요 포인트.
            GameObject bulletObj = Instantiate(bulletPrefab);
            bulletObj.SetActive(false);
            _bulletBox.PutBullet(bulletObj);
        }
        // 생산 완료 후
        PrintLog("탄환 박스를 출하 창고로 보낸다.");
        _outputStorage.Enqueue(_bulletBox);
        _bulletBox = null;
    }

    public void ClearStorage()
    {
        InputStorage = new ();
        _outputStorage.Clear();
    }

    public void SetStorage(List<ItemType> itemTypes)
    {
        foreach (var item in itemTypes)
        {
            InputStorage.Add(item, new Queue<Item>());
        }
    }

    public void SelectBullet(ItemType itemType)  
    {  // UI 에서 어떤 bullet 을 만들건지 고르는 것.
        
    }

    public void InteractBeltPut<T>(T belt) where T : ObjectOnTile, IBelt
    {
        PrintLog("벨트에서 뭔가 들어온다.");
        if (InputStorage == null || belt.item == null) return;
        PrintLog($"{belt.item.itemType} 이 들어온다. ");
        if (InputStorage.ContainsKey(belt.item.itemType) && InputStorage[belt.item.itemType].Count < _maxInput)
        {
            PrintLog($"{belt.item.itemType}를 창고에 쌓자. ");
            InputStorage[belt.item.itemType].Enqueue(belt.item);
            belt.item = null;
        }
    }
}
