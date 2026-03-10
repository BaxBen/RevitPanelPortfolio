using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.EventsArgs;
using MainRevitPanel.UI.ViewModel;
using MainRevitPanel.UI.Windows;
using MainRevitPanel.Wrapper;
using MainRevitPanel.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainRevitPanel.Commands.KR
{
    [Transaction(TransactionMode.Manual)]
    public class NumberingPileCommand : CommandBase
    {
        private ExternalEvent _externalEvent;
        private NumberingPileEventHandler _numberingHandler;

        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);

            //List<FamilyInstance> piles = GetPiles(doc);
            try
            {
                _numberingHandler = new NumberingPileEventHandler();
                _externalEvent = ExternalEvent.Create(_numberingHandler);

                var window = new NumberingPileWindow();
                var viewModel = new NumberingPileViewModel();
                window.DataContext = viewModel;
                viewModel.ApplRequested += OnApplRequested;
                window.Show();
            }
            catch (Exception ex) 
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void OnApplRequested(object sender, ApplRequestedEventArgs e)
        {
            _numberingHandler.SetParameters(e.Parameter, e.Position);
            _externalEvent.Raise();
            TaskDialog.Show("asdf", $"{e.Parameter} {string.Join(", ", e.Position)}");

        }

    }
}
