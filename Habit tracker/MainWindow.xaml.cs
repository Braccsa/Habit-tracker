using Habit_tracker.Calendar;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Habit_tracker.Classes;



namespace Habit_tracker
{
    public partial class MainWindow : Window
    {
        HabitViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new HabitViewModel();
            this.viewModel = viewModel;
            DataContext = viewModel;
            viewModel.Habits = DataManager.LoadHabitsFromJson("habits.json");
            BuildCalendarUI(DateTime.Now, viewModel.Habits.ToList());
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Dispatcher.BeginInvoke(new Action(() =>
                {
                    tb.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }
        public void BuildCalendarUI(DateTime month, List<Habit> Habits)
        {
            CalendarGrid.Children.Clear();
            MonthLabel.Text = month.ToString("MMMM yyyy", new CultureInfo("hu-HU"));// hónap év érték beállítása

            var viewModel = (HabitViewModel)this.DataContext;
            var calendarElements = CalendarGeneratorClass.GenerateCalendarUIElements(
                month,
                Habits,
                CalendarDay_Clicked // pass event handler
            );

            foreach (var element in calendarElements)
            {
                CalendarGrid.Children.Add(element);
            }
        }

        private void CalendarDay_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is CalendarDay selectedDay)
            {
                var viewModel = (HabitViewModel)this.DataContext;
                viewModel.SelectedDay = selectedDay;
                viewModel.RefreshDisplayedHabits("DaySelected");
                RefreshCalendar(DateTime.Now, viewModel.Habits.ToList());
            }
        }
        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DisplayedMonth = viewModel.DisplayedMonth.AddMonths(-1);
            BuildCalendarUI(viewModel.DisplayedMonth, ((HabitViewModel)this.DataContext).Habits.ToList());
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            viewModel.DisplayedMonth = viewModel.DisplayedMonth.AddMonths(1);
            BuildCalendarUI(viewModel.DisplayedMonth, ((HabitViewModel)this.DataContext).Habits.ToList());
        }

        public void RefreshCalendar(DateTime month, List<Habit> habits)
        {
            BuildCalendarUI(month, habits);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataManager.SaveHabitsToJson("habits.json", viewModel.Habits);
        }

    }
}