using Construction;
using UnityEngine;
using UnityEngine.UI;

public class ScoutTowerButton : MonoBehaviour
{
    Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => UnitFactory.instance.Create(Unit.SCOUT_TOWER));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
