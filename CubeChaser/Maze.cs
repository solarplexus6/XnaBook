using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CubeChaser
{
    internal class Maze
    {
        #region Constants

        public const int mazeHeight = 20;
        public const int mazeWidth = 20;

        #endregion
        #region Private fields

        public MazeCell[,] MazeCells = new MazeCell[mazeWidth,
            mazeHeight];

        private GraphicsDevice device;
        private VertexBuffer floorBuffer;
        private Color[] floorColors = new Color[2] {Color.White, Color.Gray};
        private Random rand = new Random();

        #endregion
        #region Ctors

        public Maze(GraphicsDevice device)
        {
            this.device = device;
            BuildFloorBuffer();

            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    MazeCells[x, z] = new MazeCell();
                }
            GenerateMaze();

            wallPoints[0] = new Vector3(0, 1, 0);
            wallPoints[1] = new Vector3(0, 1, 1);
            wallPoints[2] = new Vector3(0, 0, 0);
            wallPoints[3] = new Vector3(0, 0, 1);
            wallPoints[4] = new Vector3(1, 1, 0);
            wallPoints[5] = new Vector3(1, 1, 1);
            wallPoints[6] = new Vector3(1, 0, 0);
            wallPoints[7] = new Vector3(1, 0, 1);

            BuildWallBuffer();
        }

        private void BuildWallBuffer()
        {
            var wallVertexList = new
                List<VertexPositionColor>();
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    foreach (VertexPositionColor vertex
                        in BuildMazeWall(x, z))
                    {
                        wallVertexList.Add(vertex);
                    }
                }
            }
            wallBuffer = new VertexBuffer(
                device,
                VertexPositionColor.VertexDeclaration,
                wallVertexList.Count,
                BufferUsage.WriteOnly);
            wallBuffer.SetData<VertexPositionColor>(
                wallVertexList.ToArray());
        }

        private List<VertexPositionColor> BuildMazeWall(int x, int z)
        {
            var triangles = new
                List<VertexPositionColor>();
            if (MazeCells[x, z].Walls[0])
            {
                triangles.Add(CalcPoint(0, x, z, wallColors[0]));
                triangles.Add(CalcPoint(4, x, z, wallColors[0]));
                triangles.Add(CalcPoint(2, x, z, wallColors[0]));
                triangles.Add(CalcPoint(4, x, z, wallColors[0]));
                triangles.Add(CalcPoint(6, x, z, wallColors[0]));
                triangles.Add(CalcPoint(2, x, z, wallColors[0]));
            }
            if (MazeCells[x, z].Walls[1])
            {
                triangles.Add(CalcPoint(4, x, z, wallColors[1]));
                triangles.Add(CalcPoint(5, x, z, wallColors[1]));
                triangles.Add(CalcPoint(6, x, z, wallColors[1]));
                triangles.Add(CalcPoint(5, x, z, wallColors[1]));
                triangles.Add(CalcPoint(7, x, z, wallColors[1]));
                triangles.Add(CalcPoint(6, x, z, wallColors[1]));
            }

            if (MazeCells[x, z].Walls[2])
            {
                triangles.Add(CalcPoint(5, x, z, wallColors[2]));
                triangles.Add(CalcPoint(1, x, z, wallColors[2]));
                triangles.Add(CalcPoint(7, x, z, wallColors[2]));
                triangles.Add(CalcPoint(1, x, z, wallColors[2]));
                triangles.Add(CalcPoint(3, x, z, wallColors[2]));
                triangles.Add(CalcPoint(7, x, z, wallColors[2]));
            }
            if (MazeCells[x, z].Walls[3])
            {
                triangles.Add(CalcPoint(1, x, z, wallColors[3]));
                triangles.Add(CalcPoint(0, x, z, wallColors[3]));
                triangles.Add(CalcPoint(3, x, z, wallColors[3]));
                triangles.Add(CalcPoint(0, x, z, wallColors[3]));
                triangles.Add(CalcPoint(2, x, z, wallColors[3]));
                triangles.Add(CalcPoint(3, x, z, wallColors[3]));
            }
            return triangles;
        }

        private VertexPositionColor CalcPoint(
            int wallPoint, int xOffset, int zOffset, Color color)
        {
            return new VertexPositionColor(
                wallPoints[wallPoint] + new Vector3(xOffset, 0, zOffset),
                color);
        }

        #endregion
        #region Public methods

        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(floorBuffer);
                device.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    floorBuffer.VertexCount/3);

                //device.SetVertexBuffer(wallBuffer);
                //device.DrawPrimitives(
                //    PrimitiveType.TriangleList,
                //    0,
                //    wallBuffer.VertexCount/3);
            }
        }

        public void GenerateMaze()
        {
            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    MazeCells[x, z].Walls[0] = true;
                    MazeCells[x, z].Walls[1] = true;
                    MazeCells[x, z].Walls[2] = true;
                    MazeCells[x, z].Walls[3] = true;
                    MazeCells[x, z].Visited = false;
                }
            MazeCells[0, 0].Visited = true;
            EvaluateCell(new Vector2(0, 0));
        }

        #endregion
        #region Private methods

        private void BuildFloorBuffer()
        {
            var vertexList = new List<VertexPositionColor>();
            int counter = 0;
            for (int x = 0; x < mazeWidth; x++)
            {
                counter++;
                for (int z = 0; z < mazeHeight; z++)
                {
                    counter++;
                    foreach (VertexPositionColor vertex in
                        FloorTile(x, z, floorColors[counter%2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }
            floorBuffer = new VertexBuffer(
                device,
                VertexPositionColor.VertexDeclaration,
                vertexList.Count,
                BufferUsage.WriteOnly);
            floorBuffer.SetData<VertexPositionColor>(vertexList.
                                                         ToArray());
        }

        private void EvaluateCell(Vector2 cell)
        {
            var neighborCells = new List<int>();
            neighborCells.Add(0);
            neighborCells.Add(1);
            neighborCells.Add(2);
            neighborCells.Add(3);
            while (neighborCells.Count > 0)
            {
                int pick = rand.Next(0, neighborCells.Count);
                int selectedNeighbor = neighborCells[pick];
                neighborCells.RemoveAt(pick);
                Vector2 neighbor = cell;
                switch (selectedNeighbor)
                {
                    case 0:
                        neighbor += new Vector2(0, -1);
                        break;
                    case 1:
                        neighbor += new Vector2(1, 0);
                        break;
                    case 2:
                        neighbor += new Vector2(0, 1);
                        break;
                    case 3:
                        neighbor += new Vector2(-1, 0);
                        break;
                }
                if (
                    (neighbor.X >= 0) &&
                    (neighbor.X < mazeWidth) &&
                    (neighbor.Y >= 0) &&
                    (neighbor.Y < mazeHeight)
                    )
                {
                    if (!MazeCells[(int) neighbor.X, (int) neighbor.Y].
                        Visited)
                    {
                        MazeCells[
                            (int) neighbor.X,
                            (int) neighbor.Y].Visited = true;
                        MazeCells[
                            (int) cell.X,
                            (int) cell.Y].Walls[selectedNeighbor] = false;
                        MazeCells[
                            (int) neighbor.X,
                            (int) neighbor.Y].Walls[
                                (selectedNeighbor + 2)%4] = false;
                        EvaluateCell(neighbor);
                    }
                }
            }
        }

        private List<VertexPositionColor> FloorTile(
            int xOffset,
            int zOffset,
            Color tileColor)
        {
            var vList =
                new List<VertexPositionColor>();
            vList.Add(new VertexPositionColor(
                          new Vector3(0 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                          new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                          new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                          new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                          new Vector3(1 + xOffset, 0, 1 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                          new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));
            return vList;
        }

        VertexBuffer wallBuffer;
        Vector3[] wallPoints = new Vector3[8];
        Color[] wallColors = new Color[4] {
Color.Red, Color.Orange, Color.Red, Color.Orange };

        #endregion
    }
}