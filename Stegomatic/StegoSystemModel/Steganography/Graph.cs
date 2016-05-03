﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegomaticProject.StegoSystemModel.Steganography
{
    class Graph
    {
        //Constructor for the class
        public Graph()
        {

        }

        //Create list for verts and edges
        List<Vertex> VertexList = new List<Vertex>();
        List<Edge> EdgeList = new List<Edge>();
        //Create list for matched edges in the cover image
        List<Edge> MatchedEdges = new List<Edge>();
    }
}
