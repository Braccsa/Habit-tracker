using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Habit_tracker.Classes
{
    public class DataManager
    {
        public static void SaveHabitsToJson(string filePath, ObservableCollection<Habit> Habits)
        {
            try
            {
                var habitsJson = JsonSerializer.Serialize(Habits);
                File.WriteAllText(filePath, habitsJson);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ObservableCollection<Habit> LoadHabitsFromJson(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var loadedHabits = JsonSerializer.Deserialize<List<Habit>>(json);
                ObservableCollection<Habit> habits = new ObservableCollection<Habit>();

                if (loadedHabits != null)
                {
                    foreach (var habit in loadedHabits)
                    {
                        habits.Add(habit);
                    }
                    return habits;
                }
                throw new Exception("couldn't load habits");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
