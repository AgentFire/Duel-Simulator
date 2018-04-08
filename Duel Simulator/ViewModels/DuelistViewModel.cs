using PropertyChanged;
using System;

namespace AgentFire.Gwent.DuelSimulator.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public sealed class DuelistViewModel
    {
        private int _power;
        public int Power
        {
            get => _power;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Power), "Cannot be less than zero.");
                }

                _power = value;
            }
        }

        internal int Armor { get; set; }
        public string ArmorString
        {
            get => Armor == 0 ? string.Empty : Armor.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Armor = 0;
                    return;
                }

                int v = int.Parse(value);

                if (v < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Armor), "Cannot be less than zero.");
                }

                Armor = v;
            }
        }
    }
}
