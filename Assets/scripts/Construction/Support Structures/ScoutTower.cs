using Construction;

public class ScoutTower : Tower{

  private new TowerTypes TowerType {get;set;}
  public ScoutTower(int health=100, int cost=0, int capacity=5, int level=0){
    this.TowerType = TowerTypes.SUPPORT;
    this.Level = level;
    this.Health = health;
    this.Capacity = capacity;
  }

}