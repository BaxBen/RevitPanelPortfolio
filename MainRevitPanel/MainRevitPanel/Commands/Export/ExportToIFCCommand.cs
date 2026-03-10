using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.EventsArgs;
using MainRevitPanel.UI.ViewModel;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainRevitPanel.Commands.Export
{
    [Transaction(TransactionMode.Manual)]
    public class ExportToIFCCommand : CommandBase
    {
        public Dictionary<string, IFCExportConfiguration> _settings {  get; set; }
        private Document _doc;

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);
            _doc = doc;
            using (Transaction ts =new Transaction(doc, "Export to IFC"))
            {

            }
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string ifcPath = Path.Combine(desktopPath, doc.Title + ".ifc");

                // Простой диалог для подтверждения
                ExportToIFCWindow window = new ExportToIFCWindow();
                ExportToIFCViewModel viewModel = new ExportToIFCViewModel();
                viewModel.LoadData(window, IFCConfiguration().Keys.OrderBy(x=>x).ToList(), ifcPath, doc.Title);
                viewModel.ExportToIFCRequested += OnExportToIFCRequested;
                window.DataContext = viewModel; 
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private void OnExportToIFCRequested(object sender, ExportToIFCRequestedEventArgs e)
        {
            // Настраиваем опции экспорта
            IFCExportOptions exportOptions = new IFCExportOptions();

            // Базовые настройки
            exportOptions.FileVersion = _settings[e.SelectedSettings].IFCVersion;
            exportOptions.ExportBaseQuantities = _settings[e.SelectedSettings].ExportBaseQuantities;
            exportOptions.WallAndColumnSplitting = _settings[e.SelectedSettings].SplitWallsAndColumns;

            string name = Path.GetFileNameWithoutExtension(e.PathSave);
            string desktopPath = e.PathSave.Substring(0, e.PathSave.Length - Path.GetFileName(e.PathSave).Length);

            _doc.Export(desktopPath, name, exportOptions);
            TaskDialog.Show("Успешно",
                $"Модель экспортирована в IFC:\n{e.PathSave}");
        }

        public Dictionary<string, IFCExportConfiguration> IFCConfiguration()
        {
            Dictionary<string, IFCExportConfiguration> settings = new Dictionary<string, IFCExportConfiguration>();
            IFCExportConfigurationsMap configurationsMap = new IFCExportConfigurationsMap();
            configurationsMap.AddBuiltInConfigurations();
            configurationsMap.AddSavedConfigurations();
            //List<string> settings = configurationsMap.Values.Select(x=> x.Name).ToList();
            foreach (IFCExportConfiguration setting in configurationsMap.Values)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (setting != null)
                {
                    settings[setting.Name] = setting;
                }
            }
            _settings = settings;
            return settings;
        }
    }
}
