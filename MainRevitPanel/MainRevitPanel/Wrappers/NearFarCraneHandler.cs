using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Services.VK;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MainRevitPanel.Wrapper
{

    [Transaction(TransactionMode.Manual)]
    public class NearFarCraneHandler : IExternalEventHandler
    {
        public NearFarCraneWindow _window { get; set; }
        public Document _doc { get; set; }
        public Element _selectedElement { get; set; }

        /// <summary>
        ///Выполняет обновление данных в окне при изменении документа
        /// Вызывается через ExternalEvent при срабатывании DocumentChanged
        /// </summary>
        public void Execute(UIApplication app)
        {

            FamilyInstance pipeAccessory = _selectedElement as FamilyInstance;
            var connectorSet = pipeAccessory.MEPModel.ConnectorManager.Connectors;

            var runScript = true;

            int count = 0;
            foreach (Connector connectors in connectorSet)
            {
                if (connectors.MEPSystem == null)
                {
                    runScript = false;
                    TaskDialog.Show("Ошибка", "Зацикленная система");
                    break;
                }
                foreach (Connector connector in connectors.AllRefs)
                {
                    if (connector.ConnectorType != ConnectorType.End)
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    TaskDialog.Show("Ошибка", "Кран без системы");
                    runScript = false;
                    break;
                }
            }

            if (PathFinder.GetElementsSystem(_doc, _selectedElement)
                .Where(x => x.GetTypeId().IntegerValue == _selectedElement.GetTypeId().IntegerValue).ToList().Count == 1)
            {
                TaskDialog.Show("Уведомление", $"Кран в единственном экземпляре.");
                runScript = false;

            }

            if (_window != null && runScript)
            {
                var listReturn = PathFinder.Main(_selectedElement, _doc);
                NearFarCraneViewModel viewModel = new NearFarCraneViewModel();
                viewModel.LoadData(_window, listReturn[2].First(), listReturn[1], listReturn[0]);
                _window.DataContext = viewModel;
            }
            else
            {
                _window.Close();
            }

        }

        /// <summary>
        /// Просто есть
        /// </summary>
        public string GetName()
        {
            return "Second Command EventHandler Document Changed";
        }
    }
}
