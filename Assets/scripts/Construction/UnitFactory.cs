using System;
using UnityEditorInternal;
using UnityEngine;

namespace Construction{
  public enum Unit{
    SCOUT_TOWER
  }
  public class UnitFactory : MonoBehaviour{
    PlayerCamera playerCamera;
    private UnitManager _unitManager;

    private GameObject _unit = default;
    private bool _created = false;

    Vector3? groundPos;

    public static UnitFactory instance;

    void Awake() {
      if(instance == null){
        instance = this;
      }  
      else if(instance != this){
        Destroy(this);
      }
      DontDestroyOnLoad(this);
    }


    void Start()
    {
      playerCamera = GameObject.FindObjectOfType<PlayerCamera>();
      _unitManager = FindObjectOfType<UnitManager>();
    }

    void Update()
    {
      
      groundPos = playerCamera.GetMouseToGroundPosition(LayerMask.GetMask("Ground"));
      
      if ((Vector3)groundPos != null && Input.GetMouseButton(0) && !_created)
      {
        AdjustGroundPosition((Vector3)groundPos);
        _created = true; 
      }
  
      if (Input.GetMouseButton(1) && !_created)
      {
        Destroy(_unit);
      } 
      
      if(!_created){
        if (groundPos != null && _unit)
        {
          AdjustGroundPosition((Vector3)groundPos);
        }
      }
    }

    public void Create(Unit unit)
    {
      _created = false;
      _unit = default;
      switch (unit)
      {
        case Unit.SCOUT_TOWER : 
          _unit = Instantiate(_unitManager.scoutTower, Vector3.zero, Quaternion.identity);
          break;
        default:
          break;
      }
    }

    void AdjustGroundPosition(Vector3 adjustTo)
    {
      Renderer renderer = _unit.GetComponent<Renderer>();
      Collider coll = _unit.GetComponent<Collider>();
      Bounds bounds;
      if (renderer)
      {
        bounds = renderer.bounds;
      }
      else
      {
        bounds = coll.bounds;
      }
      
      _unit.transform.position = new Vector3(adjustTo.x,adjustTo.y-bounds.min.y, adjustTo.z);
    }
  }
}