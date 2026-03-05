using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.ViewModel;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Commands.Export
{
    [Transaction(TransactionMode.ReadOnly)]
    public class ExportToDWGCommand : CommandBase
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);

            List<ViewSheet> viewSheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Cast<ViewSheet>()
                .Where(s => !s.IsPlaceholder)
                .ToList();

            if (!viewSheets.Any())
            {
                TaskDialog.Show("Информация", "В проекте нет листов для экспорта.");
                return Result.Cancelled;
            }

            var dict = dicteFilter(viewSheets);

            // Показываем окно
            var window = new ExportToDWGWindow();
            ExportToDWGViewModel viewModel = new ExportToDWGViewModel();
            window.DataContext = viewModel;
            window.ShowDialog();

            return Result.Succeeded;
        }

        public Dictionary<string, List<ViewSheet>> dicteFilter(List<ViewSheet> viewSheets)
        {
            Dictionary<string, List<ViewSheet>> dict = new Dictionary<string, List<ViewSheet>> { ["Без фильтра"]= viewSheets };

            foreach (ViewSheet sheet in viewSheets)
            {
                // Получаем все параметры листа
                IList<Parameter> parameters = sheet.Parameters.Cast<Parameter>().ToList();

                foreach (Parameter param in parameters)
                {
                    if (param != null && param.HasValue)
                    {
                        string paramName = param.Definition?.Name ?? "Без имени";
                        string paramValue = GetParameterValue(param);

                        TaskDialog.Show("Параметр",
                            $"Лист: {sheet.SheetNumber} - {sheet.Name}\n" +
                            $"Параметр: {paramName}\n" +
                            $"Значение: {paramValue}");
                    }
                }
            }

            return null;
        }
        private static string GetParameterValue(Parameter parameter)
        {
            if (parameter == null || !parameter.HasValue)
                return "Нет значения";

            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.AsString() ?? "null";

                case StorageType.Integer:
                    return parameter.AsInteger().ToString();

                case StorageType.Double:
                    // Для размерных параметров
                    if (parameter.IsReadOnly)
                    {
                        return parameter.AsValueString() ?? parameter.AsDouble().ToString();
                    }
                    return parameter.AsDouble().ToString();

                case StorageType.ElementId:
                    ElementId id = parameter.AsElementId();
                    if (id != null && id.IntegerValue != -1)
                    {
                        Element elem = parameter.Element.Document.GetElement(id);
                        return elem?.Name ?? id.IntegerValue.ToString();
                    }
                    return "Нет элемента";

                default:
                    return "Неизвестный тип";
            }
        }
    }
}
