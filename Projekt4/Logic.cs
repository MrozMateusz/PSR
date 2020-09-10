using Castle.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projekt4
{
    class Logic
    {
        public int INF = 1000;
        private List<GraphElement> graphElements = new List<GraphElement>();
        public NetCom netCom;

        static void Main(string[] args)
        {
            Logic logic = new Logic();
            logic.netCom = new NetCom(logic);
            logic.netCom.init();
            logic.start();
        }

        private void start()
        {
            Console.WriteLine("Podaj definicje grafu.");
            String graf = Console.ReadLine();
            CreateGraph(graf);

          //  printResult(graphElements);
            List<GraphElement> dj = Dijkstra();

            printResult(dj);
           
            Console.ReadLine();
        }

        private void printResult(List<GraphElement> dj)
        {
            for (int o = 0; o < dj.Count; o++)
            {
                Console.WriteLine("Djikstra: " + dj[o].ToString());
            }
        }

        public String ServerJob(SimpleTCP.Message e)
        {
            return null;
        }

        public void ClientJob(SimpleTCP.Message e)
        {

        }

        private List<GraphElement> CreateGraph(string graf)
        {
            String[] graphByComa = graf.Split(',');
            for (int i = 0; i < graphByComa.Length; i++)
            {
                String pair = graphByComa[i];
                var weight = Regex.Match(pair, @"\d+").Value;
                var onlyLetters = Regex.Split(pair, @"\d+");

                GraphElement element = FindInList(onlyLetters[0]);
                if (element == null)
                {
                    element = new GraphElement(onlyLetters[0]);
                    element.AddConnection(onlyLetters[1], int.Parse(weight));
                    graphElements.Add(element);
                }
                else
                {
                    element.AddConnection(onlyLetters[1], int.Parse(weight));
                }
                GraphElement elementS = FindInList(onlyLetters[1]);
                if (element == null)
                {
                    element = new GraphElement(onlyLetters[1]);
                    graphElements.Add(elementS);
                }
            }
            //  graphElements.ForEach(element => { Console.WriteLine(element.ToString()); });
            return graphElements;
        }

        public GraphElement FindInList(String node1)
        {
            // jesli element.node1 znajduje sie juz w liscie zwróc go
            // w przeciwnym razie zwróc null
           
            return graphElements.Find(element => element.node1 == node1);
           
        }

        public class Cost
        {
            public GraphElement node;
            public int cost;

            public Cost(GraphElement node, int cost)
            {
                this.node = node;
                this.cost = cost;
            }
        }

        public List<GraphElement> Dijkstra()
        {
            List<GraphElement> Q = new List<GraphElement>(graphElements);
            List<GraphElement> S = new List<GraphElement>();
            List<Cost> d = new List<Cost>();
            List<GraphElement> p = new List<GraphElement>(new GraphElement[graphElements.Count]);

            GraphElement source = graphElements.First();
            graphElements.ForEach(element => d.Add(new Cost(element, INF)));

            Cost initialCost = FindCost(d, source);
            initialCost.cost = 0;

            while (Q.Count > 0)
            {
                Cost lowestCost = FindLowestCost(d, Q);
                GraphElement graphElement = lowestCost.node;

                Q.Remove(graphElement);
                S.Add(graphElement);

                foreach (Pair<string, int>  connection in graphElement.connections)
                {
                    GraphElement neighbour = graphElements.Find(element => element.node1 == connection.First);
                    int weight = connection.Second;

                    if (!Q.Contains(neighbour)) continue;

                    Cost neighbourCost = FindCost(d, neighbour);
                    if (neighbourCost.cost > lowestCost.cost + weight)
                    {
                        neighbourCost.cost = lowestCost.cost + weight;
                        //trzeba znalezc odpowiednik neighbour w tablicy p
                        p[graphElements.FindIndex(element => neighbour.node1 == element.node1)] = neighbour;
                    }

                }
            }
            return p;
        }


        private Cost FindCost(List<Cost> d, GraphElement neighbour)
        {
            return d.Find(cost => cost.node.node1 == neighbour.node1);
        }

        private Cost FindLowestCost(List<Cost> d, List<GraphElement> Q)
        {
            Cost result = d[0];
            for (int l = 0; l < d.Count; l++)
            {
                if (result.cost > d[l].cost && Q.Find(element => element.node1 == d[l].node.node1) != null)
                {
                    result = d[l];
                }
            }
            return result;
        }
    }

    class GraphElement
    {
        public String node1;
        public List<Pair<String, int>> connections;

        public GraphElement(String node1)
        {
            this.node1 = node1;
            this.connections = new List<Pair<String, int>>();
        }

        public void AddConnection(String node, int weight)
        {
            this.connections.Add(new Pair<String,int>(node, weight));
        }

        public String ToString()
        {
            String r = null;
            for (int t = 0; t < connections.Count; t++)
            {
               r = "[ node: " + node1 + " connections: " + connections[t] + "]";
            }
            return r;
        }
    }


}
