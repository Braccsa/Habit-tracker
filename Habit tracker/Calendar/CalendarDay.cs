using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Habit_tracker.Classes;

namespace Habit_tracker
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public List<Habit> Habits { get; set; } = new();

    }
}
