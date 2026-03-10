using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MainRevitPanel.Wrappers
{
    public class NumberingPileEventHandler : IExternalEventHandler
    {
        private bool Parameter { get; set; }
        private List<bool> Position { get; set; }
        private List<FamilyInstance> _elements;

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = app.ActiveUIDocument.Document;
            if (Parameter)
            {
                IList<Reference> reference = uidoc.Selection.PickObjects(ObjectType.Element, new FilterPile());
                _elements = reference.Select(x => doc.GetElement(x.ElementId) as FamilyInstance).ToList();
            }
            else
            {
                _elements = GetPiles(doc);
            }
            Numbering(Position[0], Position[1], Position[2]);
        }

        public string GetName()
        {
            return " ";
        }

        public void SetParameters(bool parameter, List<bool> position)
        {
            Parameter = parameter;
            Position = position;
        }

        private void Numbering(bool param0, bool param1, bool param2)
        {
            var dict_StructuralFoundation = new Dictionary<FamilyInstance, List<double>>();
            /// Остановился Здесь 
            var list_StructuralFoundation = new List<List<double>>();
            foreach (FamilyInstance element in _elements)
            {
                dict_StructuralFoundation[element] = GetDoubleXYZ(element);
                list_StructuralFoundation.Add(GetDoubleXYZ(element));
            }
            list_StructuralFoundation.OrderBy(list=> list[0]);

        }

        /// <summary>
        /// Собирает все сваи в документе по параметру "Описание" содержащему "Свая"
        /// </summary>
        private List<FamilyInstance> GetPiles(Document doc)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .Cast<FamilyInstance>();

            List<FamilyInstance> piles = new List<FamilyInstance>();

            foreach (FamilyInstance instance in collector)
            {
                ElementType type = instance.Document.GetElement(instance.GetTypeId()) as ElementType;
                Parameter descParam = type?.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                if (descParam != null && descParam.AsString() != null)
                {
                    string description = descParam.AsString().ToLower();
                    if (description.Contains("свая"))
                    {
                        piles.Add(instance);
                    }
                }
            }

            return piles;
        }

        private class FilterPile : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if (elem.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

        public List<double> GetDoubleXYZ(FamilyInstance elem)
        {
            LocationPoint locationPoint = elem.Location as LocationPoint;
            var point = locationPoint.Point;
            List<double> list = new List<double> { point.X, point.Y };
            return list;
        }
    }
}
