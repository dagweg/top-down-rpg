using UnityEngine;

namespace Construction
{
  public class Tower{
    protected int Capacity { get; set;}
    protected int Health { get; set;}
    protected int Level {get;set;}
    protected int Cost {get; set;}
    protected enum TowerTypes { DEFENSIVE, OFFENSIVE, SUPPORT};
    protected TowerTypes TowerType {get; set;}
  }
}