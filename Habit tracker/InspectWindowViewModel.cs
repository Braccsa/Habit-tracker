using Habit_tracker.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habit_tracker
{
    public class InspectWindowViewModel : INotifyPropertyChanged
    {
        //uppon closing should refresh calendar
        private Habit? _inspectedHabit;
        public ObservableCollection<HabitType> HabitTypes { get; }
        public ObservableCollection<Frequency> Frequencies { get; }
        public bool IsModified { get; private set; } = false;

        public Habit InspectedHabit
        {
            get { return _inspectedHabit; }
            set
            {
                _inspectedHabit = value;
                OnPropertyChanged(nameof(InspectedHabit));
                IsModified = true;
            }
        }


        public InspectWindowViewModel(Habit inspectedHabit, ObservableCollection<HabitType> HabitTypes, ObservableCollection<Frequency> frequencies)
        {
            this.InspectedHabit = inspectedHabit;
            IsModified = false;
            this.HabitTypes = HabitTypes;
            this.Frequencies = frequencies;
        }

        public InspectWindowViewModel()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
