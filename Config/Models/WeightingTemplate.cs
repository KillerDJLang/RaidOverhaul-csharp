namespace ROConfig.Models;

public class WeightingTemplate
{
    public int SwitchToggle { get; set; } = 2;
    public int DoorUnlock { get; set; } = 12;
    public int KeycardUnlock { get; set; } = 1;
    public int DoorEventRangeMinimum { get; set; } = 1;
    public int DoorEventRangeMaximum { get; set; } = 3;
    public int DamageEvent { get; set; } = 40;
    public int AirdropEvent { get; set; } = 110;
    public int BlackoutEvent { get; set; } = 80;
    public int JokeEvent { get; set; } = 40;
    public int HealEvent { get; set; } = 120;
    public int ArmorEvent { get; set; } = 140;
    public int SkillEvent { get; set; } = 60;
    public int MetabolismEvent { get; set; } = 60;
    public int MalfunctionEvent { get; set; } = 40;
    public int TraderEvent { get; set; } = 25;
    public int BerserkEvent { get; set; } = 40;
    public int WeightEvent { get; set; } = 40;
    public int MaxLLEvent { get; set; } = 5;
    public int LockdownEvent { get; set; } = 10;
    public int ArtilleryEvent { get; set; } = 10;
    public int RandomEventRangeMinimum { get; set; } = 5;
    public int RandomEventRangeMaximum { get; set; } = 25;
}
