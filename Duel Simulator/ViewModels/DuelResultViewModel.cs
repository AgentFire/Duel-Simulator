namespace AgentFire.Gwent.DuelSimulator.ViewModels
{
    public sealed class DuelResultViewModel
    {
        public int TotalPowerImpact { get; set; }

        public DuelistResultViewModel Initiator { get; set; }
        public DuelistResultViewModel Defendant { get; set; }
    }
}
