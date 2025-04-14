using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habit_tracker.Classes
{
    public enum HabitType
    {
        health,
        sport,
        hobby,
        study,
        other
    }
    public enum Frequency { once, daily, weekly, monthly }

    public class Habit
    {
        private string title;
        private HabitType type;
        private DateTime firstDate;
        private DateTime lastDate;
        private Frequency frequency;
        public bool completed;
        public List<DateTime> CompletedDates { get; set; } = new List<DateTime>();

        public Habit(string title, HabitType habitType)
        {
            Title = title;
            Type = habitType;
            FirstDate = DateTime.Now;
            frequency = Frequency.once;
            completed = false;
            CompletedDates = new List<DateTime>();
            lastDate = firstDate;
        }

        public Habit(string title, HabitType type, DateTime firstDate, Frequency frequency)
        {
            Title = title;
            Type = type;
            FirstDate = firstDate;
            Frequency = frequency;
            completed = false;
            CompletedDates = new List<DateTime>();
            lastDate = firstDate; 
        }
        public Habit() //required for deserialization
        {
        }
        public void MarkCompleted(DateTime day)
        {
            if (!CompletedDates.Contains(day.Date))
            {
                CompletedDates.Add(day.Date);
            }
        }

        public void UnmarkCompleted(DateTime day)
        {
            CompletedDates.Remove(day.Date);
        }
        public bool IsCompletedOn(DateTime day)
        {
            return CompletedDates.Contains(day.Date);
        }

        public Frequency Frequency { get => frequency; set => frequency = value; }
        public DateTime FirstDate 
        { 
            get => firstDate; 
            set 
            {
                if (lastDate > value)
                {
                    LastDate = value;
                    firstDate = value;
                }
                else
                {
                    firstDate = value;
                }

            }
        }
        public HabitType Type { get => type; set => type = value; }
        public string Title { get => title; set => title = value; }
        public DateTime LastDate 
        { 
            get => lastDate;
            set
            {
                if (Frequency == Frequency.once)
                {
                    lastDate = FirstDate;
                }
                else
                {
                    lastDate = value;
                }
            }
        }
    }
}
