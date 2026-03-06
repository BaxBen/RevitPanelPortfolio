using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MainRevitPanel.UI.Windows;
using MainRevitPanel.Wrapper;
using MainRevitPanel.Services.VK;
using MainRevitPanel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using MainRevitPanel.Services;

namespace MainRevitPanel.Commands.VK
{
    [Transaction(TransactionMode.ReadOnly)]
    public class NearFarCraneCommand : CommandBase
    {
        private static ExternalEvent _externalEventDocumentChanged;
        private static NearFarCraneHandler _handlerDocumentChanged = new NearFarCraneHandler();
        private static bool _isWindowOpen = false;
        public bool _isSubscription = true;

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = GetUIApplication(commandData);
            UIDocument uidoc = GetUIDocument(commandData);
            Document doc = GetDocument(commandData);

            if (_isWindowOpen) return Result.Cancelled;
            _isWindowOpen = true;

            if (_isSubscription)
                doc.Application.DocumentChanged += OnDocumentChanged;


            Reference reference = null;

            try
            {
                reference = uidoc.Selection.PickObject(ObjectType.Element, new FilterPipeAccessory());
                var runScript = true;
                Element elem = doc.GetElement(reference.ElementId);

                FamilyInstance pipeAccessory = elem as FamilyInstance;
                var connectorSet = pipeAccessory.MEPModel.ConnectorManager.Connectors;

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

                if (runScript && PathFinder.GetElementsSystem(doc, elem)
                    .Where(x => x.GetTypeId().IntegerValue == elem.GetTypeId().IntegerValue).ToList().Count == 1)
                {
                    TaskDialog.Show("Уведомление", $"Кран в единственном экземпляре.");
                    runScript = false;

                }

                List<List<string>> listReturn = null;

                if (runScript)
                {
                    listReturn = PathFinder.Main(elem, doc);
                    _handlerDocumentChanged._doc = doc;
                    _handlerDocumentChanged._selectedElement = elem;
                    _externalEventDocumentChanged = ExternalEvent.Create(_handlerDocumentChanged);
                }
                else
                {
                    _isWindowOpen = false;
                    _isSubscription = false;
                }

                if (listReturn != null)
                {
                    PrintWindow(uiapp, listReturn);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                _isWindowOpen = false;
                _isSubscription = false;
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Обрабатывает событие DocumentChanged
        /// </summary>
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            if (!_isSubscription)
            {
                e.GetDocument().Application.DocumentChanged -= OnDocumentChanged;
                return;
            }
            _externalEventDocumentChanged.Raise();
        }

        /// <summary>
        /// Обрабатывает событие закрытия окна и обновляет состояние флагов подписки и открытого окна
        /// </summary>
        private void OnClosed(bool param)
        {
            _isWindowOpen = param;
            _isSubscription = param;
        }

        /// <summary>
        /// Отображает модальное окно команды, устанавливая его владельцем главное окно Revit
        /// </summary>
        private void PrintWindow(UIApplication uiapp, List<List<string>> listReturn)
        {
            var revitHandle = uiapp.MainWindowHandle;

            var window = new NearFarCraneWindow();
            var viewModel = new NearFarCraneViewModel();
            viewModel.LoadData(window, listReturn[2].First(), listReturn[1], listReturn[0]);
            _handlerDocumentChanged._window = window;
            WindowInteropHelper helper = new WindowInteropHelper(window);
            helper.Owner = revitHandle;
            window.Topmost = false;

            window.DataContext = viewModel;
            window.Show();
            window.Closed += (s, e) => OnClosed(false);
        }
    }
}
