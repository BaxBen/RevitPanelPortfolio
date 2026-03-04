using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRevitPanel.Services
{
    public class GraphPathFinder
    {
        /// <summary>
        /// Ищет путь от start до end в списке graph
        /// </summary>
        public static List<int> FindPath(Dictionary<int, List<int>> graph, int start, int end)
        {
            Queue<List<int>> queue = new Queue<List<int>>();
            HashSet<int> visited = new HashSet<int>();

            queue.Enqueue(new List<int> { start });
            visited.Add(start);

            while (queue.Count > 0)
            {
                List<int> path = queue.Dequeue();
                int currentNode = path[path.Count - 1];

                if (graph.ContainsKey(currentNode))
                {
                    foreach (int neighbor in graph[currentNode])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            List<int> newPath = new List<int>(path);
                            newPath.Add(neighbor);

                            if (neighbor == end)
                            {
                                return newPath;
                            }

                            visited.Add(neighbor);
                            queue.Enqueue(newPath);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Выводит сообщение TaskDialog путь из переданого списка
        /// </summary>
        public static void PrintPath(List<int> pathList)
        {
            if (pathList == null)
            {
                TaskDialog.Show("print", "Путь не найден");
                return;
            }
            TaskDialog.Show("print", $"{string.Join(" -> ", pathList)}");
        }
    }
}
