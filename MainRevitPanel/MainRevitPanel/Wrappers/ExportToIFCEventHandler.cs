using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using MainRevitPanel.Services;
using MainRevitPanel.UI.ViewModel;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MainRevitPanel.Wrappers
{
    public class ExportToIFCEventHandler : IExternalEventHandler
    {
        public IFCExportConfiguration _settings {  get; set; }
        public string _name { get; set; }
        public string _desktopPath { get; set; }
        public bool _createModel { get; set; }

        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument.Document;
            try
            {
                using (Transaction ts = new Transaction(doc, "Export to IFC"))
                {
                    ts.Start();
                    IFCExportOptions exportOptions = new IFCExportOptions();

                    exportOptions.FileVersion = _settings.IFCVersion;
                    exportOptions.ExportBaseQuantities = _settings.ExportBaseQuantities;
                    exportOptions.WallAndColumnSplitting = _settings.SplitWallsAndColumns;
                    
                    doc.Export(_desktopPath, _name, exportOptions);
                    ts.Commit();

                    TaskDialog.Show("Успешно",
                        $"Модель экспортирована в IFC:\n{_desktopPath}");

                    if (_createModel)
                    {
                        GipVision GIP = new GipVision();
                        GIP.LoadData(Path.Combine(_desktopPath, _name + ".ifc"), "");
                        if (GIP.apiToken?.Any() ?? false)
                        {
                            GIP.Main();
                        }
                        else
                        {
                            var window = new AddKeyGipVisionWindow();
                            var viewModel = new AddKeyGipVisionViewModel();
                            viewModel.LoadData(window, GIP);
                            window.DataContext = viewModel;
                            window.ShowDialog();
                            if (GIP.apiToken?.Any() ?? false)
                            {
                                GIP.Main();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"{ex}");
            }
        }


        public string GetName()
        {
            return " ";
        }
    }
}
