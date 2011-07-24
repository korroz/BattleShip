using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNABattleShipFrontEnd
{
   public class BoardGrid
   {
      private const float CELL_SIZE = 100;

      private VertexPositionColor[] _vertexList;
      private short[] _vertexIndexList;

      public VertexPositionColor[] VertexList { get { return _vertexList; } }
      public short[] VertexIndexList { get { return _vertexIndexList; } }

      public BoardGrid(int xSize, int ySize)
      {
         int maxVertices = (xSize + 1) + (ySize + 1);
         float boardYSize = ySize * CELL_SIZE;
         float boardXSize = xSize * CELL_SIZE;
         Vector3 origin = new Vector3(boardXSize / 2.0f, boardYSize / 2.0f, 0.0f);

         _vertexList = new VertexPositionColor[maxVertices * 2];
         _vertexIndexList = new short[maxVertices * 2];

         int firstIndex, secondIndex;
         for (int x = 0; x < xSize + 1; x++)
         {
            firstIndex = x * 2;
            secondIndex = (x * 2) + 1;

            Vector3 linePlane = Vector3.Negate(origin);
            linePlane.X += x * CELL_SIZE;

            _vertexList[firstIndex] = new VertexPositionColor(linePlane, Color.White);
            _vertexIndexList[firstIndex] = (short)firstIndex;

            linePlane.Y += boardYSize;
            _vertexList[secondIndex] = new VertexPositionColor(linePlane, Color.White);
            _vertexIndexList[secondIndex] = (short)secondIndex;
         }

         int indexOffset = (xSize * 2) + 2;
         for (int y = 0; y < ySize + 1; y++)
         {
            firstIndex = indexOffset + (y * 2);
            secondIndex = indexOffset + ((y * 2) + 1);

            Vector3 linePlane = Vector3.Negate(origin);
            linePlane.Y += y * CELL_SIZE;

            _vertexList[firstIndex] = new VertexPositionColor(linePlane, Color.White);
            _vertexIndexList[firstIndex] = (short)firstIndex;

            linePlane.X += boardXSize;
            _vertexList[secondIndex] = new VertexPositionColor(linePlane, Color.White);
            _vertexIndexList[secondIndex] = (short)secondIndex;
         }
      }

      public BoardGrid(int xSize, int ySize, Matrix transform)
      {
         int lineCount = (xSize + 1) + (ySize + 1);
         List<Line> lineList = new List<Line>(lineCount);


      }
   }
}
