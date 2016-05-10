﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegomaticProject.StegoSystemModel.Steganography
{
    class Graph
    {
        //Constructor for the class
        public Graph(List<Pixel> pixelList, int pixelsNeeded)
        {
            this.PixelList = pixelList;
            this.PixelsNeeded = pixelsNeeded;
        }
        public List<Pixel> PixelList;
        public List<EncodeVertex> EncodeVertexList = new List<EncodeVertex>();
        public List<DecodeVertex> DecodeVertexList = new List<DecodeVertex>();
        public List<Edge> EdgeList = new List<Edge>();
        public List<Edge> MatchedEdges = new List<Edge>();
        public int PixelsNeeded { get; }

        public void ConstructGraph(int pixelsNeeded, List<byte> secretMessage)
        {
            Console.WriteLine("Starting with constructing vertices");
            Console.ReadKey();
            ConstructVertices(pixelsNeeded, secretMessage);
            Console.WriteLine("Constructed a bunch of vertices      -       " + EncodeVertexList.Count);
            Console.ReadKey();
            CheckIfMatched();
            ConstructEdges();
            CheckIfMatched();
            Console.WriteLine("Before calcGraph");
            Console.WriteLine("VertexList:    " + EncodeVertexList.Count);
            Console.WriteLine("Edgelist:    " + EdgeList.Count);
            Console.WriteLine("MatchedEdgelist:    " + MatchedEdges.Count);
            Console.ReadKey();
            CalcGraphMatching();
            Console.WriteLine("SUCCESS!!!!");
            Console.WriteLine("VertexList:    " + EncodeVertexList.Count);
            Console.WriteLine("Edgelist:    " + EdgeList.Count);
            Console.WriteLine("MatchedEdgelist:    " + MatchedEdges.Count);

            CheckIfMatched();

        }

        public List<Pixel> ModifyGraph()
        {
            Console.WriteLine("Before swapping pixels");
            Console.ReadKey();
            PixelSwap();
            Console.WriteLine("Before modifying");
            Console.ReadKey();
            PixelModify();

            return PixelList;
        }

        private void ConstructVertices(int pixelsNeeded, List<byte> secretMessage)
        {
            int counter = 0;
            for (int i = 0; i < pixelsNeeded; i += GraphTheoryBased.SamplesVertexRatio)
            {
                EncodeVertex vertex = new EncodeVertex(secretMessage[counter], PixelList[i], PixelList[i + 1], PixelList[i + 2]); //this is hardcoded and can maybe rewritten by using a delegate.
                EncodeVertexList.Add(vertex);
                counter++;
            }
        }
        private void ConstructVertices(int pixelsNeeded)
        {
            int counter = 0;
            for (int i = 0; i < pixelsNeeded; i += GraphTheoryBased.SamplesVertexRatio)
            {
                DecodeVertex decodeVertex = new DecodeVertex(PixelList[i], PixelList[i + 1], PixelList[i + 2]); //this is hardcoded and can maybe rewritten by using a delegate.
                DecodeVertexList.Add(decodeVertex);
                counter++;
            }
        }
        private void ConstructEdges()
        {
            byte lowestWeight = GraphTheoryBased.MaxEdgeWeight;
            byte edgeWeight;
            short amountOfEdges = 0;

            //need double for loop, to check every vertex with every other vertex
            for (int i = 0; i < EncodeVertexList.Count; i++)
            {
                for (int j = 0; j < EncodeVertexList.Count; j++)
                {
                    if (i != j && EncodeVertexList[i].Active == true && EncodeVertexList[j].Active == true) //don't want to compare a vertex with itself
                    {
                        bool b = ConstructASingleEdge(EncodeVertexList[i], EncodeVertexList[j], out edgeWeight); //return true if an edge was created
                        if (b == true)
                        {
                            amountOfEdges++;
                            if (amountOfEdges == 10000) //Hardcoded limit for edges per vert
                            {
                                break;
                            }
                            if (edgeWeight <= lowestWeight)
                            {
                                lowestWeight = edgeWeight;
                            }
                        }

                    }
                }
                EncodeVertexList[i].LowestEdgeWeight = lowestWeight;
                EncodeVertexList[i].NumberOfEdges = amountOfEdges;
                EncodeVertexList[i].Active = false; //after examining a single vertex, it will be deactivated since all of the possible edges already have been evaluated, and therefore there is no need to look at this particular vertex again.
            }
        }

        private bool ConstructASingleEdge(EncodeVertex vertex1, EncodeVertex vertex2, out byte lowestWeight)
        {
            byte weight = GraphTheoryBased.MaxEdgeWeight;
            Edge tempEdge = new Edge(null, null, null, null, 0);
            for (int i = 0; i < GraphTheoryBased.SamplesVertexRatio; i++)
            {
                for (int j = 0; j < GraphTheoryBased.SamplesVertexRatio; j++)
                {
                    if (vertex1.PixelsForThisVertex[i].EmbeddedValue == vertex2.TargetValues[j] &&
                        vertex2.PixelsForThisVertex[j].EmbeddedValue == vertex1.TargetValues[i] &&
                        CalculateWeightForOneEdge(vertex1.PixelsForThisVertex[i], vertex2.PixelsForThisVertex[j]) <= GraphTheoryBased.MaxEdgeWeight)
                    {
                        //Only have to make 1 edge, for two vertices, but there could potentially be more than 1 pr. 2 vertices
                        if (CalculateWeightForOneEdge(vertex1.PixelsForThisVertex[i],
                                vertex2.PixelsForThisVertex[j]) <= weight)
                        {
                            weight = CalculateWeightForOneEdge(vertex1.PixelsForThisVertex[i],
                                vertex2.PixelsForThisVertex[j]);
                            tempEdge = new Edge(vertex1, vertex2, vertex1.PixelsForThisVertex[i], vertex2.PixelsForThisVertex[j], weight);
                        }
                    }
                }
            }

            if (tempEdge.EdgeWeight != 0) //edgeweight will never be zero, because a pixel cannot have an embeddedvalue that's equivalent with it's targetvalue
            {
                EdgeList.Add(tempEdge);
                lowestWeight = weight;
                return true;
            }
            lowestWeight = 0;
            return false;
        }

        private byte CalculateWeightForOneEdge(Pixel pixel1, Pixel pixel2)
        {
            byte weight = (byte)(Math.Abs(pixel1.Color.R - pixel2.Color.R) + Math.Abs(pixel1.Color.G - pixel2.Color.G) +
                     Math.Abs(pixel1.Color.B - pixel2.Color.B));
            return weight;
        }

        public void CheckIfMatched() //this will be called multiple times. 
        {
            for (int i = 0; i < EncodeVertexList.Count; i++)
            {
                if (EncodeVertexList[i].PartOfSecretMessage != EncodeVertexList[i].VertexValue)
                {
                    EncodeVertexList[i].Active = true;
                }
            }
        }

        private void SortVertexListByEdgeAndWeight()
        {
            EncodeVertexList.OrderBy(x => x.NumberOfEdges).ThenBy(x => x.LowestEdgeWeight);
        }

        private void CalcGraphMatching()
        {
            SortVertexListByEdgeAndWeight();

            List<Edge> tempMatched = new List<Edge>();

            foreach (EncodeVertex vert in EncodeVertexList)
            {
                if (vert.Active == true && vert.NumberOfEdges > 0)
                {
                    List<Edge> InternalEdgeList = new List<Edge>();
                    foreach (Edge edge in EdgeList)
                    {
                        if (edge.VertexOne == vert || edge.VertexTwo == vert)
                        {
                            InternalEdgeList.Add(edge);
                        }
                    }

                    List<Edge> SortedInternalList = InternalEdgeList.OrderBy(o => o.EdgeWeight).ToList();

                    //foreach (var item in InternalEdgeList)
                    //{
                    //    Console.WriteLine(item.ToString());
                    //}
                    //Console.ReadKey();

                    Edge M = SortedInternalList.First();
                    /*HVIS ALT ER MATCHET BLIVER DER IKKE OPRETTET
                    KANTER, OG DERFOR KASTER DENNE EN NULLEXCEPTION!!!*/


                    //Edge FoundInGlobalList = EdgeList.FirstOrDefault(i => i == M);

                    tempMatched.Add(M);
                }
            }


            MatchedEdges = DeleteDuplicatesInList(tempMatched);

        }

        private List<Edge> DeleteDuplicatesInList(List<Edge> list)
        {
            return list.Distinct().ToList();
        }


        private void PixelSwap()
        {
            //for (int i = 0; i < MatchedEdges.Count; i++)
            //{
            //    TradePixelValues(MatchedEdges[i].VertexPixelOne, MatchedEdges[i].VertexPixelTwo);

            //    //set vertices, that are now correct, to false
            //    MatchedEdges[i].VertexOne.Active = false;
            //    MatchedEdges[i].VertexTwo.Active = false;
            //}
            //foreach (Edge edge in MatchedEdges)
            //{
            //    Console.WriteLine(edge.ToString());
            //}

            foreach (Edge edge in MatchedEdges)
            {
                TradePixelValues(edge.VertexPixelOne, edge.VertexPixelTwo);

                edge.VertexOne.Active = false;
                edge.VertexTwo.Active = false;
            }

        }
        //Method for helping pixels trade values
        private void TradePixelValues(Pixel pixelOne, Pixel pixelTwo)
        {
            int tempPosX = pixelOne.PosX;
            int tempPosY = pixelOne.PosY;

            pixelOne.PosX = pixelTwo.PosX;
            pixelOne.PosY = pixelTwo.PosY;

            pixelTwo.PosX = tempPosX;
            pixelTwo.PosY = tempPosY;
        }

        private void PixelModify()
        {
            for (int i = 0; i < EncodeVertexList.Count; i++)
            {
                if (EncodeVertexList[i].Active == true)
                {
                    HelpMethodPixelModify(EncodeVertexList[i]);
                }
            }
        }
        private void HelpMethodPixelModify(EncodeVertex vertex) //always uses first sample in vertex
        {
            //print this vertex
            Console.WriteLine(vertex.PixelsForThisVertex[0].EmbeddedValue + "\n" + vertex.TargetValues[0] + "\n\n");
            //Console.ReadKey();
            int localDifference = 0;
            while ((Math.Abs(vertex.PixelsForThisVertex[0].EmbeddedValue + localDifference)) % GraphTheoryBased.Modulo != vertex.TargetValues[0])
            {
                if (vertex.PixelsForThisVertex[0].Color.R <= 127) //always red channel
                {
                    localDifference++;
                }
                else
                {
                    localDifference--;
                }
                Console.WriteLine(localDifference);

            }
            vertex.PixelsForThisVertex[0].ColorDifference = localDifference;
        }


    }
}