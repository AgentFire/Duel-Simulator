using AgentFire.Gwent.DuelSimulator.Components;
using AgentFire.Gwent.DuelSimulator.ViewModels;
using MahApps.Metro.Controls;
using PropertyChanged;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AgentFire.Gwent.DuelSimulator
{
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : MetroWindow
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        public ICommand LeftClickCommand => new RelayCommand(_ =>
        {
            ShowSelf();
        });
        public ICommand SwitchCommand => new RelayCommand(_ =>
        {
            DuelistViewModel temp = Initiator;
            Initiator = Defendant;
            Defendant = temp;
        });

        public DuelistViewModel Initiator { get; set; } = new DuelistViewModel() { Power = 8, Armor = 3 };
        public DuelistViewModel Defendant { get; set; } = new DuelistViewModel() { Power = 11, Armor = 0 };
        public DuelResultViewModel DuelResult { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            KeyboardHook.KeyEvent += KeyboardHook_KeyEvent;

            Duel();
        }

        private void KeyboardHook_KeyEvent(Key key, bool wasPressed)
        {
            if (key == Key.D && Keyboard.IsKeyDown(Key.LeftCtrl) && wasPressed && Process.GetProcessesByName("Gwent").SingleOrDefault()?.MainWindowHandle == GetForegroundWindow())
            {
                ShowSelf();
            }
        }

        private void ShowSelf()
        {
            Opacity = 0.9;
            Activate();
        }

        private void win_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Opacity = 0;
                WindowState = WindowState.Normal;
            }
        }

        private void Duel()
        {
            DuelistResultViewModel initiator = new DuelistResultViewModel(Initiator);
            DuelistResultViewModel defendant = new DuelistResultViewModel(Defendant);

            var a = initiator;
            var b = defendant;

            int totalDamage = 0;

            while (!a.IsDead && !b.IsDead)
            {
                int dmg = a.Power;

                if (b.Armor > 0)
                {
                    if (b.Armor > dmg)
                    {
                        b.Armor -= dmg;
                        dmg = 0;
                    }
                    else
                    {
                        dmg -= b.Armor;
                        b.Armor = 0;
                    }
                }

                if (dmg > 0)
                {

                    if (b.Power < dmg)
                    {
                        totalDamage += b.Power;
                        b.Power = 0;
                        continue;
                    }

                    b.Power -= dmg;
                    totalDamage += dmg;
                }

                var t = a;
                a = b;
                b = t;
            }

            DuelResult = new DuelResultViewModel()
            {
                Initiator = initiator,
                Defendant = defendant,
                TotalPowerImpact = totalDamage
            };
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Duel();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
