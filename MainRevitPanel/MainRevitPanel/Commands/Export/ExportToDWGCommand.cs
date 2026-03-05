using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MainRevitPanel.Commands.Export
{
    [Transaction(TransactionMode.Manual)]
    public class ExportToDWGCommand : CommandBase
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);

            try
            {
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

                ExportToDWGWindow window = new ExportToDWGWindow(viewSheets);

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    var selectedViewSheet = window.SelectedViewSheet;

                    var dialog = new System.Windows.Forms.FolderBrowserDialog()
                    {
                        Description = "Выберите папку для сохранения",
                        ShowNewFolderButton = true
                    };

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportToDWG(doc, selectedViewSheet, dialog.SelectedPath);

                        TaskDialog.Show("Успешно",
                            $"Экспортировано {selectedViewSheet.Count} спецификаций\n" +
                            $"Файл сохранен: {dialog.SelectedPath}");
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public void ExportToDWG(Document doc, List<ViewSheet> selectedViewSheet, string filePath)
        {
            //В будущем добавлю возможность настроить параметры экспорта 
            using (Transaction ts = new Transaction(doc, "run process"))
            {
                ts.Start();
                DWGExportOptions options = new DWGExportOptions();
                options.FileVersion = ACADVersion.R2013;


                doc.Export(filePath, " ", selectedViewSheet.Select(x => x.Id).ToList(), options);


                ts.Commit();
            }

        }
    }
}
