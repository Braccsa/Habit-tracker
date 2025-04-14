using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Habit_tracker.Classes;

namespace Habit_tracker
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        HabitViewModel ViewModel { get; set; }
        public DetailsWindow(Habit habit,  ObservableCollection<HabitType> HabitTypes, ObservableCollection<Frequency> Frequencies, HabitViewModel vm)
        {
            InitializeComponent();
            var viewModel = new InspectWindowViewModel(habit, HabitTypes, Frequencies);
            DataContext = viewModel;
            ViewModel = vm;

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; 
            this.Close();
            ViewModel.RefreshDisplayedHabits();
            
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
            ViewModel.RefreshDisplayedHabits();
        }
       
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var vm = this.DataContext as InspectWindowViewModel;

            if (FocusManager.GetFocusedElement(this) is FrameworkElement focusedElement)//force update elements before closing so we can refresh the display
            {
                BindingExpression binding = focusedElement.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();
            }

            if (vm != null && vm.IsModified)
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }
}
