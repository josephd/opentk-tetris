using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using Texture = System.Int32;

namespace Tetris
{
  class FieldRenderer : GameComponent
  {
    public class Cell
    {
      public bool inUse = false;
      public TetraminoColor color;
    }

    Vector3 position;
    Vector3[] corners = new Vector3[]
    {
      new Vector3( .5f,  .5f, 0f),
      new Vector3(-.5f,  .5f, 0f),
      new Vector3(-.5f, -.5f, 0f),
      new Vector3( .5f, -.5f, 0f)
    };
    float[][] texCoords = new float[][]
    {
      new float[] { 0.0f, 0.0f },
      new float[] { 1.0f, 0.0f },
      new float[] { 1.0f, 1.0f },
      new float[] { 0.0f, 1.0f }
    };
    Cell[,] shownCells;
    Cell[,] committedCells;
    int width;
    int height;

    public Cell this[int x, int y]
    {
      get
      {
	return shownCells[x, y];
      }
      set
      {
        shownCells[x, y] = value;
      }
    }

    public FieldRenderer(GameWindow window, Vector3 position, int width, int height) : base(window)
    {
      this.position = Vector3.Add(position, new Vector3(-(width / 2) + .5f, -(height / 2) + .5f, 0.0f));
      shownCells = new Cell[width, height];
      committedCells = new Cell[width, height];
      this.width = width;
      this.height = height;
      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
	{
          committedCells[x, y] = new Cell();
	  shownCells[x, y] = new Cell();
	}
    }

    public void Clear()
    {
      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
          shownCells[x, y].inUse = false;
    }

    public void CopyCommit()
    {
      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
	  if (shownCells[x, y].inUse)
	  {
	    committedCells[x, y].inUse = shownCells[x, y].inUse;
	    committedCells[x, y].color = shownCells[x, y].color;
	  }
    }

    protected override void DoUpdate(FrameEventArgs e) { }

    protected override void DoDraw(FrameEventArgs e)
    {
      GL.BindTexture(TextureTarget.Texture2D, ResourceCommons.Cell);
      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
	{
          GL.Begin(BeginMode.Quads);
          RenderCube(Vector3.Add(new Vector3((float)x, (float)y, 0), position));
          GL.End();
	}
      GL.BindTexture(TextureTarget.Texture2D, ResourceCommons.Block);
      for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
	{
          if (committedCells[x, y].inUse)
	  {
            GL.Begin(BeginMode.Quads);
            RenderCube(Vector3.Add(new Vector3((float)x, (float)y, -1f), position));
            GL.End();
	  }
          if (shownCells[x, y].inUse)
	  {
            GL.Begin(BeginMode.Quads);
            RenderCube(Vector3.Add(new Vector3((float)x, (float)y, -1f), position));
            GL.End();
	  }
	}
    }

    void RenderCube(Vector3 cubePos)
    { //TODO:Use VBO
      for (int i = 0; i < 4; i++)
      {
        GL.TexCoord2(texCoords[i]);
        GL.Vertex3(Vector3.Add(corners[i], cubePos));
      }
    }
  }
}