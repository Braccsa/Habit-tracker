using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Text.Json;
using Habit_tracker.Classes;


namespace Habit_tracker
{

    public class HabitViewModel : INotifyPropertyChanged
    {
        private Habit _currentHabit;
        private ObservableCollection<Habit> _habits;//list of all saved habits
        private ObservableCollection<Habit> _displayedHabitsList;
        private CalendarDay _selectedDay;
        public ObservableCollection<HabitType> HabitTypes { get; }
        public ObservableCollection<Frequency> Frequencies { get; }
        private Habit _selectedHabit;

        private DateTime _displayedMonth = DateTime.Now;

        public ICommand AddHabitCommand { get; } // erre tud hivatkozni a gomb, ezt össze tudjuk konstruktorban kötni a függvénnyel
        public ICommand OpenDetailsCommand { get; }
        public ICommand OrderCommand { get; }
        public ICommand DeleteHabitCommand { get; }
        public ICommand ResetVievCommand { get; }
        private bool isForever;
        public bool IsForever
        {
            get => isForever;
            set
            {
                if (isForever != value)
                {
                    isForever = value;
                    OnPropertyChanged(nameof(IsForever));
                    IsEndDateEnabled = !value;

                    if (value)
                    {
                        CurrentHabit.LastDate = new DateTime(2100, 12, 31);
                    }
                }
            }
        }

        private bool isEndDateEnabled = true;

        public bool IsEndDateEnabled
        {
            get => isEndDateEnabled;
            set
            {
                if (isEndDateEnabled != value)
                {
                    isEndDateEnabled = value;
                    OnPropertyChanged(nameof(IsEndDateEnabled));
                }
            }
        }

        public DateTime DisplayedMonth
        {
            get { return _displayedMonth; }
            set
            {
                _displayedMonth = value;
                OnPropertyChanged(nameof(DisplayedMonth));
            }
        }
        public CalendarDay SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged(nameof(SelectedDay));
            }
        }
        public ObservableCollection<Habit> Habits 
        {
            get => _habits;
            set
            {
                _habits = value;
                OnPropertyChanged(nameof(Habits));
            }
        }
        public ObservableCollection<Habit> DisplayedHabitsList //habits to be displayed in the checkbox
        { //Should be updated, when filter is changed, new item added(defult view, clear filter), Day is opened (current days habit list) 
            get => _displayedHabitsList;
            set
            {
                _displayedHabitsList = value;
                OnPropertyChanged(nameof(DisplayedHabitsList));
            }
        }
        public Habit CurrentHabit //habit that is currently edited in the mvvm and will be added to the list
        {
            get => _currentHabit;
            set
            {
                    _currentHabit = value;
                    OnPropertyChanged(nameof(CurrentHabit));
            }
        }
        public Habit SelectedHabit //habit that is selected in the listbox, can be used to inspect, edit, etc. 
        {
           
            get => _selectedHabit;
            set
            {
                _selectedHabit = value;
                OnPropertyChanged(nameof(SelectedHabit));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public HabitViewModel()
        {
            Habits = new ObservableCollection<Habit>();
            _currentHabit = new Habit("Habit Title", HabitType.other);
            HabitTypes = new ObservableCollection<HabitType>(Enum.GetValues(typeof(HabitType)).Cast<HabitType>());
            Frequencies = new ObservableCollection<Frequency>(Enum.GetValues(typeof(Frequency)).Cast<Frequency>());
            DisplayedHabitsList = new ObservableCollection<Habit>(Habits);

            AddHabitCommand = new RelayCommand(AddHabit);//belerakjuk az addhabitcommandba
            OpenDetailsCommand = new RelayCommand(OpenDetailsWindow);//új ablak nyitás
            OrderCommand = new RelayCommand(OrderHabit);
            DeleteHabitCommand = new RelayCommand(DeleteSelectedHabit);
            ResetVievCommand = new RelayCommand(RefreshDisplayedHabits);


        }
        public void AddHabit()
        {
            if(CurrentHabit.Frequency==Frequency.once &&
                CurrentHabit.FirstDate > CurrentHabit.LastDate)
            {
                CurrentHabit.LastDate = CurrentHabit.FirstDate;
            }
            if (isForever)
            {
                CurrentHabit.LastDate = (new DateTime(2100, 12, 31));
            }
            Habits.Add(_currentHabit);

            
            CurrentHabit = new Habit("Habit Title", HabitType.other)//reset current habit
            {
                FirstDate = DateTime.Now,
                Frequency = Frequency.daily
                
            };

            if (isForever) 
            {
                CurrentHabit.LastDate=(new DateTime(2100, 12, 31));
            }

            RefreshDisplayedHabits();

            var mainWindow = (MainWindow)Application.Current.MainWindow;//resets the calendar
            mainWindow.RefreshCalendar(DateTime.Now, Habits.ToList());

        }
        public void DeleteSelectedHabit()
        {
            if (SelectedHabit != null && Habits.Contains(SelectedHabit))
            {
                Habits.Remove(SelectedHabit);
                SelectedHabit = null;

                RefreshDisplayedHabits();

                var mainWindow = (MainWindow)Application.Current.MainWindow;//resets the calendar
                mainWindow.RefreshCalendar(DateTime.Now, Habits.ToList());
            }
        }
        public void RefreshDisplayedHabits(string command = "Default")//by default shows all habits
        {
            DisplayedHabitsList.Clear();
            switch (command)
            {
                case "DaySelected":
                    foreach (var habit in SelectedDay.Habits)
                    {
                        DisplayedHabitsList.Add(habit);
                    }
                    break;
                case "Default":
                default:
                    foreach (var habit in Habits)
                    {
                        DisplayedHabitsList.Add(habit);
                    }
                    break;
            }
        }
        public void RefreshDisplayedHabits()//override with no parameters
        {
            RefreshDisplayedHabits("Default");
        }

        private void OrderHabit()
        {
            var sorted = Habits.OrderBy(h => h.FirstDate).ToList();

            Habits.Clear();
            foreach (var habit in sorted)
            {
                Habits.Add(habit);
            }
        }
        private void OpenDetailsWindow()
        {
            if (SelectedHabit != null)
            {
                var window = new DetailsWindow(SelectedHabit, HabitTypes, Frequencies, this);
                bool? result = window.ShowDialog(); // Open as dialog so we can return value

                if (result == true)
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.RefreshCalendar(DateTime.Now, Habits.ToList());

                }
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            public RelayCommand(Action execute)//elmentjük a függvényt
            {
                _execute = execute;
            }
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                _execute();
            }
        }
    }
}
