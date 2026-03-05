using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace MainRevitPanel.Services.VK
{
    public class PathFinder
    {
        private static Document _doc;
        // Словарь ключь индекс секции, значение все элементы секции.
        public static Dictionary<int, HashSet<int>> _sectionElements;
        // Словарь ключь индекс секции, значение длина секции секции.
        public static Dictionary<int, double> _sectionLength;

        public static Dictionary<int, HashSet<int>> _connectorElements = new Dictionary<int, HashSet<int>>();

        /// <summary>
        /// Находит все элементы в контуре и возварает HasSet<Element>
        /// </summary>
        public static HashSet<Element> GetElementsSystem(Document doc, Element elem)
        {

            FamilyInstance pipeAccessory = elem as FamilyInstance;
            var connectorSet = pipeAccessory.MEPModel.ConnectorManager.Connectors;

            HashSet<int> pipingNetwork = new HashSet<int> { elem.Id.IntegerValue };

            foreach (Connector connector in connectorSet)
            {
                foreach (Connector item in connector.AllRefs)
                {
                    if (item.ConnectorType == ConnectorType.End)
                    {
                        var mepSystem = item.MEPSystem;
                        for (int y = 0; y < mepSystem.SectionsCount; y++)
                        {
                            for (int i = 0; i < mepSystem.SectionsCount; i++)
                            {
                                HashSet<int> listElementIds = mepSystem.GetSectionByIndex(i).GetElementIds().Select(x => x.IntegerValue).ToHashSet();
                                if (listElementIds.Intersect(pipingNetwork).Any())
                                {
                                    foreach (int elementId in listElementIds)
                                    {
                                        pipingNetwork.Add(elementId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return pipingNetwork.Select(x => doc.GetElement(new ElementId(x))).ToHashSet();
        }

        /// <summary>
        /// Добавляет в словари _sectionLength длину секций и _sectionElements сами секции
        /// </summary>
        private static void GetSectionElements(Element elem)
        {
            FamilyInstance pipeAccessory = elem as FamilyInstance;
            var connectorSet = pipeAccessory.MEPModel.ConnectorManager.Connectors;
            Dictionary<int, HashSet<int>> dict = new Dictionary<int, HashSet<int>>();
            Dictionary<int, double> dictSectionLength = new Dictionary<int, double>();

            foreach (Connector connectors in connectorSet)
            {
                HashSet<object> hasSet = new HashSet<object>();
                foreach (Connector connector in connectors.AllRefs)
                {
                    if (connector.ConnectorType == ConnectorType.End)
                    {
                        var section = connector.MEPSystem.SectionsCount;
                        for (int i = 0; i < section; i++)
                        {
                            if (!dict.ContainsKey(i))
                            {
                                //TaskDialog.Show($"{i}", $"{string.Join(", ", connector.MEPSystem.GetSectionByIndex(i).GetElementIds().Select(x => x.IntegerValue).ToHashSet())}");
                                dict[i] = connector.MEPSystem.GetSectionByIndex(i).GetElementIds().Select(x => x.IntegerValue).ToHashSet();
                                dictSectionLength[i] = UnitUtils.ConvertFromInternalUnits(connector.MEPSystem.GetSectionByIndex(i).TotalCurveLength, UnitTypeId.Millimeters);
                            }
                        }
                    }
                }
            }

            _sectionLength = dictSectionLength;
            _sectionElements = dict;
        }

        /// <summary>
        /// Возвращает номер секции в зависимости от переданного элемента
        /// </summary>
        private static int GetSectionForElement(Element elem)
        {
            foreach (var row in _sectionElements)
            {
                var key = row.Key;
                var values = row.Value;
                if (values.Contains(elem.Id.IntegerValue))
                {
                    return key;
                }
            }
            return -1;
        }

        /// <summary>
        /// Возвращает словарь с конекторами Key(Коннектор) : Value(Трубы)
        /// </summary>
        private static Dictionary<int, List<int>> GetArrayPath()
        {

            var dictPathConnection = new Dictionary<int, List<int>>();
            foreach (var row1 in _sectionElements)
            {
                var key1 = row1.Key;
                var values1 = row1.Value;
                var list = new List<int>();
                foreach (var row2 in _sectionElements)
                {
                    var key2 = row2.Key;
                    var values2 = row2.Value;
                    if (key1 == key2) continue;

                    if (values1.Intersect(values2).Any())
                    {
                        list.Add(key2);
                    }
                }
                dictPathConnection[key1] = list;
            }

            return dictPathConnection;

        }

        /// <summary>
        /// Возвращает словарь в зависимости от переданной секции Key(Коннектор) : Value(Трубы)
        /// </summary>
        private static Dictionary<int, List<int>> GetArrayPathSection(int section)
        {
            var dict = new Dictionary<int, List<int>>();
            var listConnectors = _sectionElements[section]
                .Select(x => _doc.GetElement(new ElementId(x)))
                .Where(x => x is FamilyInstance)
                .Select(x => x as FamilyInstance)
                .Select(x => x.MEPModel.ConnectorManager)
                .ToList();

            foreach (var connectorSet in listConnectors)
            {
                HashSet<int> hasSet = new HashSet<int>();
                foreach (Connector connectors in connectorSet.Connectors)
                {
                    foreach (Connector connector in connectors.AllRefs)
                    {
                        if (connector.ConnectorType == ConnectorType.End)
                        {
                            hasSet.Add(connector.Owner.Id.IntegerValue);
                        }
                    }
                    //hasSet.Add(connectors.Owner.Id.IntegerValue);
                }
                //TaskDialog.Show($"PROVERKA {connectorSet.Owner.Id.IntegerValue}", $"{string.Join(", ", hasSet)}");
                _connectorElements[connectorSet.Owner.Id.IntegerValue] = hasSet;
                dict[connectorSet.Owner.Id.IntegerValue] = hasSet.ToList();
            }

            var dictReturn = new Dictionary<int, List<int>>();
            foreach (var row1 in dict)
            {
                var key1 = row1.Key;
                var values1 = row1.Value;
                var list = new List<int>();
                foreach (var row2 in dict)
                {
                    var key2 = row2.Key;
                    var values2 = row2.Value;
                    if (key1 == key2) continue;

                    if (values1.Intersect(values2).Any())
                    {
                        list.Add(key2);
                    }
                }
                dictReturn[key1] = list;
                //TaskDialog.Show($"Connector - {key1}", $"Connectors - {string.Join(", ", list)}");
            }
            return dictReturn;

        }

        /// <summary>
        /// Основная исполняемая функция
        /// </summary>
        public static List<List<string>> Main(Element elem, Document doc)
        {
            _doc = doc;
            GetSectionElements(elem);

            // Словарь ключь Id крана, значение все элементы участвующие в пути.
            var dictValveElements = new Dictionary<int, HashSet<int>>();
            // Словарь ключь Id крана, значение длина пути.
            var dictValveLength = new Dictionary<int, double>();

            var arraPath = GetArrayPath();

            int start = GetSectionForElement(elem);

            List<Element> listValve = GetElementsSystem(doc, elem)
                .Where(x => x.GetTypeId().IntegerValue == elem.GetTypeId().IntegerValue && x.Id.IntegerValue != elem.Id.IntegerValue)
                .ToList();


            try
            {
                foreach (Element valve in listValve)
                {
                    int end = GetSectionForElement(valve);
                    List<int> paths = new List<int>();
                    HashSet<int> hasSet = new HashSet<int>();
                    var referencePath = GraphPathFinder.FindPath(arraPath, start, end);

                    if (start != end)
                    {
                        var countEnd = _sectionElements[end]
                            .Select(x => doc.GetElement(new ElementId(x)))
                            .Where(x => x.GetTypeId().IntegerValue == elem.GetTypeId().IntegerValue).Count();
                        var countStart = _sectionElements[start]
                            .Select(x => doc.GetElement(new ElementId(x)))
                            .Where(x => x.GetTypeId().IntegerValue == elem.GetTypeId().IntegerValue).Count();

                        foreach (var path in referencePath)
                        {
                            if (path == end && countEnd > 1)
                            {
                                continue;
                            }
                            else if (path == start && countStart > 1) 
                            {
                                continue;
                            }

                            if (path == end && countEnd == 1)
                            {
                                var listItems = _sectionElements[end]
                                .Intersect(_sectionElements[referencePath[referencePath.Count - 2]]).ToList();
                                var connectorTee = listItems.First();
                                hasSet.UnionWith(HasSetForPipesInOneSystem(end, connectorTee, valve.Id.IntegerValue));
                                continue;
                            }
                            if (path == start && countStart == 1)
                            {
                                var listItems = _sectionElements[start]
                                .Intersect(_sectionElements[referencePath[1]]).ToList();
                                var connectorTee = listItems.First();
                                hasSet.UnionWith(HasSetForPipesInOneSystem(start, connectorTee, elem.Id.IntegerValue));
                                continue;
                            }
                            foreach (int elementId in _sectionElements[path])
                            {
                                hasSet.Add(elementId);
                            }
                        }

                        if (countEnd > 1)
                        {
                            var listItems = _sectionElements[end]
                                .Intersect(_sectionElements[referencePath[referencePath.Count - 2]]).ToList();
                            var connectorTee = listItems.First();
                            paths = GraphPathFinder.FindPath(GetArrayPathSection(end), connectorTee, valve.Id.IntegerValue);

                            paths.Reverse();
                            foreach (int path in paths)
                            {

                                if (paths.IndexOf(path) > 0)
                                {
                                    var index = paths.IndexOf(path) - 1;
                                    var x = paths[index];
                                    var colection = _connectorElements[path].Intersect(_connectorElements[x]);
                                    hasSet.Add(path);
                                    hasSet.UnionWith(colection);
                                }
                            }
                        }
                        if (countStart > 1)
                        {
                            var listItems = _sectionElements[start]
                                .Intersect(_sectionElements[referencePath[1]]).ToList();
                            var connectorTee = listItems.First();
                            paths = GraphPathFinder.FindPath(GetArrayPathSection(start), connectorTee, elem.Id.IntegerValue);

                            paths.Reverse();
                            foreach (int path in paths)
                            {

                                if (paths.IndexOf(path) > 0)
                                {
                                    var index = paths.IndexOf(path) - 1;
                                    var x = paths[index];
                                    var colection = _connectorElements[path].Intersect(_connectorElements[x]);
                                    hasSet.Add(path);
                                    hasSet.UnionWith(colection);
                                }
                            }
                        }

                        dictValveElements[valve.Id.IntegerValue] = hasSet;


                    }
                    if (start == end)
                    {
                        dictValveElements[valve.Id.IntegerValue] = HasSetForPipesInOneSystem(start, elem.Id.IntegerValue, valve.Id.IntegerValue);
                    }
                    //PrintPath(paths);
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
            }

            foreach (var row in dictValveElements)
            {
                double lengthPath = 0;
                foreach (var id in row.Value)
                {
                    Element element = doc.GetElement(new ElementId(id));
                    if (element is Pipe)
                    {
                        double length = UnitUtils.ConvertFromInternalUnits(element.LookupParameter("Длина").AsDouble(), UnitTypeId.Millimeters);
                        lengthPath += length;
                    }
                }
                dictValveLength[row.Key] = Math.Round(lengthPath, 2);
            }

            double min = dictValveLength.Values.Min();
            double max = dictValveLength.Values.Max();

            List<string> listMin = new List<string>();
            List<string> listMax = new List<string>();
            List<string> listSelectedElement = new List<string>() { elem.Id.IntegerValue.ToString() };

            foreach (var row in dictValveLength)
            {
                var id = row.Key;
                var length = row.Value;
                string stringpPath = lineBreak(dictValveElements[id].ToList());


                if (min == length)
                {
                    listMin.Add($"Кран ID: {id}, Длина пути: {length} мм. \nПуть: {stringpPath}");
                }
                if (max == length)
                {
                    listMax.Add($"Кран ID: {id},  Длина пути: {length} мм. \nПуть: {stringpPath}");
                }
            }

            var listReturn = new List<List<string>> { listMin, listMax, listSelectedElement };



            return listReturn;
        }

        /// <summary>
        /// Возвращает HasSet<int> путь от startValve до endValve в секции section
        /// </summary>
        private static HashSet<int> HasSetForPipesInOneSystem(int section, int startValve, int endValve)
        {
            var hasSet = new HashSet<int>();
            var paths = GraphPathFinder.FindPath(GetArrayPathSection(section), startValve, endValve);
            foreach (var path in paths)
            {
                foreach (var id in _connectorElements[path])
                {
                    if ((path == paths.Last() || path == paths.First()) && paths.Count > 2)
                    {
                        hasSet.Add(path);
                    }
                    else if (paths.Count == 2)
                    {
                        hasSet = new HashSet<int>(_connectorElements[paths.First()].Intersect(_connectorElements[paths.Last()]).ToList());
                    }
                    else
                    {
                        hasSet.Add(id);
                    }
                }
                hasSet.Add(path);
            }
            return hasSet;
        }

        /// <summary>
        /// Возвращает string с переносом слов в зависимости от количество слов в строке countString
        /// </summary>
        private static string lineBreak(List<int> list, int countString = 4)
        {
            string stringpPath = string.Empty;

            for (int i = 0; i < list.Count; i++)
            {
                stringpPath += list[i];

                if (i == list.Count - 1)
                {
                }
                else if ((i + 1) % countString == 0)
                {
                    stringpPath += ",\n";
                }
                else
                {
                    stringpPath += ", ";
                }
            }

            return stringpPath;
        }

    }
}
