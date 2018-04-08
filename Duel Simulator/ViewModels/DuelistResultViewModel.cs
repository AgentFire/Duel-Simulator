namespace AgentFire.Gwent.DuelSimulator.ViewModels
{
    public sealed class DuelistResultViewModel
    {
        public DuelistViewModel Initial { get; }

        public int Power { get; set; }
        public int Armor { get; set; }

        public DuelistResultViewModel(DuelistViewModel initial)
        {
            Initial = initial;
            Power = initial.Power;
            Armor = initial.Armor;
        }

        public bool IsDead => Power == 0;
        public bool HadInitialArmorAndStillHasIt => Initial.Armor > 0 && Armor > 0;
    }
}
