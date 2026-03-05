using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Commands.Export
{
    [Transaction(TransactionMode.Manual)]
    public class ExportSchedulesToExcelCommand : CommandBase
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = GetDocument(commandData);

            try
            {
                // Собираем все спецификации
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                List<ViewSchedule> schedules = collector
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>()
                    .Where(v => !v.IsTemplate && v.Name != "Sheet List")
                    .ToList();

                ExportSchedulesToExcelWindow window = new ExportSchedulesToExcelWindow(schedules);

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    var selectedSchedules = window.SelectedSchedules;

                    var dialog = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        Title = "Сохранить отчет",
                        FileName = $"Спецификации_{DateTime.Now:yyyy-MM-dd}.xlsx"
                    };

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ExportToExcel(doc, selectedSchedules, dialog.FileName);

                        TaskDialog.Show("Успешно",
                            $"Экспортировано {selectedSchedules.Count} спецификаций\n" +
                            $"Файл сохранен: {dialog.FileName}");
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
        /// <summary>
        /// Создает файл Excel, Заполняет данные из выбраных спецификаций
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="schedules"></param>
        /// <param name="filePath"></param>
        private void ExportToExcel(Document doc, List<ViewSchedule> schedules, string filePath)
        {
            int numbetr = 0;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                foreach (var schedule in schedules)
                {
                    //numbetr Это костыль что бы все спецификации могли записываться, так как есть ограничения по длине символов, не больше 30
                    numbetr++;
                    var worksheet = package.Workbook.Worksheets.Add($"{numbetr} " + GetValidSheetName(schedule.Name));

                    var tableData = schedule.GetTableData();
                    var section = tableData.GetSectionData(SectionType.Body);

                    for (int i = 0; i < section.NumberOfColumns; i++)
                    {
                        var cell = worksheet.Cells[1, i + 1];
                        cell.Value = schedule.GetCellText(SectionType.Body, 0, i);
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    for (int row = 1; row < section.NumberOfRows; row++)
                    {
                        for (int col = 0; col < section.NumberOfColumns; col++)
                        {
                            var cellValue = schedule.GetCellText(SectionType.Body, row, col);
                            worksheet.Cells[row + 1, col + 1].Value = cellValue;
                        }
                    }

                    worksheet.Cells.AutoFitColumns();
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }
        /// <summary>
        /// Проверяет длину имени и наличие запрещеных символов, если она больше 30 символов, то обрезает
        /// </summary>
        /// <param name="Имя выбранной спецификации"></param>
        /// <returns></returns>
        private string GetValidSheetName(string name)
        {
            if (name.Length > 30)
                name = name.Substring(0, 30);

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c.ToString(), "");
            }

            return name;
        }
    }
}
