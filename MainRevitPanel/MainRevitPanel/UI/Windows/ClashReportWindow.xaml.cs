using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MainRevitPanel.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClashReportWindow.xaml
    /// </summary>
    public partial class ClashReportWindow : Window
    {
        private List<ClashResult> _clashes;
        private Document _doc;
        private UIDocument _uidoc;

        public ClashReportWindow(List<ClashResult> clashes, Document doc, UIDocument uidoc)
        {
            InitializeComponent();

            _clashes = clashes;
            _doc = doc;
            _uidoc = uidoc;

            SummaryText.Text = $"Найдено пересечений: {clashes.Count}";
            ClashesGrid.ItemsSource = clashes;
        }

        private void HighlightButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClashesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пересечение из списка!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                var selectedClash = ClashesGrid.SelectedItem as ClashResult;

                List<ElementId> idsToSelect = new List<ElementId>
                {
                    new ElementId(selectedClash.DuctId),
                    new ElementId(selectedClash.PipeId)
                };

                _uidoc.Selection.SetElementIds(idsToSelect);

                _uidoc.ShowElements(idsToSelect);

                StatusBar.Text = $"Выделены: {selectedClash.DuctName} и {selectedClash.PipeName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выделении: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = "JSON Files|*.json",
                    Title = "Сохранить отчет",
                    FileName = $"ОтчетПересечений_{DateTime.Now:HH-mm_dd-MM-yyyy}.json"
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var exportData = new
                    {
                        ExportDate = DateTime.Now,
                        DocumentName = _doc.Title,
                        ClashesCount = _clashes.Count,
                        Clashes = _clashes
                    };

                    string json = JsonConvert.SerializeObject(exportData, Formatting.Indented);

                    File.WriteAllText(dialog.FileName, json);

                    StatusBar.Text = $"Отчет сохранен: {dialog.FileName}";

                    MessageBox.Show($"Отчет успешно сохранен!\n{dialog.FileName}",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
