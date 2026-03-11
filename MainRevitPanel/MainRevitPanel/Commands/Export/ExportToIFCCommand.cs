using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.EventsArgs;
using MainRevitPanel.UI.ViewModel;
using MainRevitPanel.UI.Windows;
using MainRevitPanel.Wrapper;
using MainRevitPanel.Wrappers;
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
        private ExternalEvent _externalEvent;
        private ExportToIFCEventHandler _handler;
        private Document _doc;

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);
            _doc = doc;
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string ifcPath = Path.Combine(desktopPath, doc.Title + ".ifc");

                _handler = new ExportToIFCEventHandler();
                _externalEvent = ExternalEvent.Create(_handler);

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
            // Базовые настройки
            string name = Path.GetFileNameWithoutExtension(e.PathSave);
            string desktopPath = e.PathSave.Substring(0, e.PathSave.Length - Path.GetFileName(e.PathSave).Length);

            _handler._name = name;
            _handler._desktopPath = desktopPath;

            _handler._settings = _settings[e.SelectedSettings];
            _handler._createModel = e.SelectedCreateModel;
            _externalEvent.Raise();
        }

        public Dictionary<string, IFCExportConfiguration> IFCConfiguration()
        {
            Dictionary<string, IFCExportConfiguration> settings = new Dictionary<string, IFCExportConfiguration>();
            IFCExportConfigurationsMap configurationsMap = new IFCExportConfigurationsMap();
            configurationsMap.AddBuiltInConfigurations();
            configurationsMap.AddSavedConfigurations();
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
