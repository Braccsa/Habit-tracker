using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Habit_tracker.Classes;

namespace Habit_tracker.Calendar
{
    public class CalendarGeneratorClass
    {
        public static List<CalendarDay> GenerateCalendarDays(DateTime month, List<Habit> habits)
        {
            var days = new List<CalendarDay>();

            // Start from the 1st of the month
            var startDay = new DateTime(month.Year, month.Month, 1);

            // Generate exactly 35 consecutive days
            for (int i = 0; i < 35; i++)
            {
                var currentDay = startDay.AddDays(i);

                var dayHabits = habits
                    .Where(h => IsHabitScheduledOn(h, currentDay))
                    .ToList();

                days.Add(new CalendarDay
                {
                    Date = currentDay,
                    Habits = dayHabits
                });
            }

            return days;
        }

        public static List<UIElement> GenerateCalendarUIElements(
            DateTime month,
            List<Habit> allHabits,
            MouseButtonEventHandler clickHandler)
        {
            var calendarDays = GenerateCalendarDays(month, allHabits);
            var uiElements = new List<UIElement>();

            foreach (var day in calendarDays)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(4),
                    Margin = new Thickness(1),
                    Cursor = Cursors.Hand,
                    Tag = day
                };

                if (clickHandler != null)
                    border.MouseLeftButtonUp += clickHandler;

                var stack = new StackPanel();

                var dateText = new TextBlock
                {
                    Text = day.Date.Day.ToString(),
                    FontWeight = FontWeights.Bold
                };

                stack.Children.Add(dateText);

                foreach (var habit in day.Habits)
                {
                    var habitText = new TextBlock
                    {
                        Text = habit.Title,
                        FontSize = 10,
                        Margin = new Thickness(0, 2, 0, 0),
                        Foreground = habit.IsCompletedOn(day.Date) ? Brushes.Green : Brushes.DarkRed,
                        Cursor = Cursors.Hand,
                        Tag = new Tuple<Habit, DateTime>(habit, day.Date)
                    };

                    habitText.MouseLeftButtonUp += (s, e) =>
                    {
                        var tag = (Tuple<Habit, DateTime>)((TextBlock)s).Tag;
                        var h = tag.Item1;
                        var d = tag.Item2;

                        if (h.IsCompletedOn(d))
                            h.UnmarkCompleted(d);
                        else
                            h.MarkCompleted(d);

                        // Update the color based on the new state
                        habitText.Foreground = h.IsCompletedOn(d) ? Brushes.Green : Brushes.DarkRed;
                    };

                    stack.Children.Add(habitText);
                }

                bool allCompleted = day.Habits.All(h => h.IsCompletedOn(day.Date)) && day.Habits.Any();
                if (allCompleted)
                {
                    border.Background = new SolidColorBrush(Color.FromRgb(200, 255, 200)); // light green
                }
                else
                {
                    border.Background = Brushes.White;
                }


                border.Child = stack;
                uiElements.Add(border);
            }

            return uiElements;
        }


        private static bool IsHabitScheduledOn(Habit habit, DateTime day)
        {
            // Exclude if outside the date range
            if (day < habit.FirstDate.Date || day > habit.LastDate.Date)
                return false;

            switch (habit.Frequency)
            {
                case Frequency.daily:
                    return true; // All days between FirstDate and LastDate
                case Frequency.weekly:
                    return (day - habit.FirstDate).Days % 7 == 0;
                case Frequency.monthly:
                    return day.Day == habit.FirstDate.Day;
                case Frequency.once:
                    return day.Date == habit.FirstDate.Date;
                default:
                    return false;
            }
        }



    }
}
