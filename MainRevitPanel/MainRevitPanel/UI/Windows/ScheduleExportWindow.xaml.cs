using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainRevitPanel.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для ExportSchedulesToExcelWindow.xaml
    /// </summary>
    public partial class ExportSchedulesToExcelWindow : Window
    {
        private List<ViewSchedule> _allSchedules;

        public List<ViewSchedule> SelectedSchedules { get; private set; }

        public ExportSchedulesToExcelWindow(List<ViewSchedule> schedules)
        {
            InitializeComponent();
            _allSchedules = schedules;
            SchedulesListBox.ItemsSource = schedules;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SchedulesListBox.SelectAll();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SelectedSchedules = SchedulesListBox.SelectedItems.Cast<ViewSchedule>().ToList();

            if (SelectedSchedules.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну спецификацию!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
