using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using MainRevitPanel.Base;
using MainRevitPanel.UI.Models;
using MainRevitPanel.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace MainRevitPanel.Commands.VK
{
    [Transaction(TransactionMode.Manual)]
    public class ClashDetectionTool : CommandBase
    {
        public override Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = GetUIDocument(commandData);
            Document doc = GetDocument(commandData);
            try
            {
                ProgressBarWindow progress = new ProgressBarWindow();
                progress.Show();

                List<ClashResult> clashes = new List<ClashResult>();

                FilteredElementCollector ductCollector = new FilteredElementCollector(doc);
                IList<Duct> ducts = ductCollector
                    .OfClass(typeof(Duct))
                    .Cast<Duct>()
                    .ToList();

                FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
                IList<Pipe> pipes = pipeCollector
                    .OfClass(typeof(Pipe))
                    .Cast<Pipe>()
                    .ToList();

                progress.MaxValue = ducts.Count;
                progress.CurrentValue = 0;

                foreach (Duct duct in ducts)
                {
                    progress.CurrentValue++;
                    progress.UpdateProgress();

                    Solid ductSolid = GetElementSolid(duct);
                    if (ductSolid == null) continue;

                    foreach (Pipe pipe in pipes)
                    {
                        if (duct.LevelId != pipe.LevelId) continue;

                        Solid pipeSolid = GetElementSolid(pipe);
                        if (pipeSolid == null) continue;

                        Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(
                            ductSolid,
                            pipeSolid,
                            BooleanOperationsType.Intersect);

                        if (intersection != null && intersection.Volume > 0.001) // > 1 см^3
                        {
                            clashes.Add(new ClashResult
                            {
                                DuctId = duct.Id.IntegerValue,
                                PipeId = pipe.Id.IntegerValue,
                                DuctName = duct.Name,
                                PipeName = pipe.Name,
                                Volume = intersection.Volume,
                                Location = (duct.Location as LocationPoint)?.Point
                            });
                        }
                    }
                }

                progress.Close();

                if (clashes.Count > 0)
                {
                    ShowClashesWindow(clashes, doc, uidoc);
                }
                else
                {
                    TaskDialog.Show("Результат", "Пересечений не найдено!");
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        private Solid GetElementSolid(Element element)
        {
            Options options = new Options
            {
                ComputeReferences = true,
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = false
            };

            GeometryElement geomElement = element.get_Geometry(options);
            if (geomElement == null) return null;

            List<Solid> solids = new List<Solid>();

            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is Solid solid && solid.Volume > 0)
                {
                    solids.Add(solid);
                }
                else if (geomObj is GeometryInstance instance)
                {
                    GeometryElement instanceGeom = instance.GetInstanceGeometry();
                    foreach (GeometryObject obj in instanceGeom)
                    {
                        if (obj is Solid solid2 && solid2.Volume > 0)
                        {
                            solids.Add(solid2);
                        }
                    }
                }
            }

            if (solids.Count == 0) return null;

            Solid combinedSolid = solids.First();
            for (int i = 1; i < solids.Count; i++)
            {
                combinedSolid = BooleanOperationsUtils.ExecuteBooleanOperation(
                    combinedSolid,
                    solids[i],
                    BooleanOperationsType.Union);
            }

            return combinedSolid;
        }

        private void ShowClashesWindow(List<ClashResult> clashes, Document doc, UIDocument uidoc)
        {
            ClashReportWindow window = new ClashReportWindow(clashes, doc, uidoc);
            window.ShowDialog();
        }
    }
}
