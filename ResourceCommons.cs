using GameFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using Texture = System.Int32;

using GLPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Tetris
{
  static class ResourceCommons
  {
    const string RESOURCE_DIR = "assets";
    const string TEXTURES_DIR = "textures";
    const string MODELS_DIR = "models";

    public static Texture Cell;
    public static Dictionary<TetraminoColor, MeshRenderer> Blocks = new Dictionary<TetraminoColor, MeshRenderer>();

    static XmlSerializer modelSerializer = new XmlSerializer(typeof(Mesh));

    public static void Load()
    {
      LoadTexture(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), TEXTURES_DIR), "cell.png"), out Cell);
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockCyan.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Cyan, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockYellow.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Yellow, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockPurple.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Purple, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockGreen.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Green, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockRed.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Red, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockBlue.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Blue, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(".", RESOURCE_DIR), MODELS_DIR), "blockOrange.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Orange, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
    }

    public static void Unload()
    {
      GL.DeleteTextures(1, ref Cell);
    }

    public static void LoadTexture(string filename, out Texture id)
    {
      if (String.IsNullOrEmpty(filename))
        throw new ArgumentException(filename);
      id = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, id);
      Bitmap bmp = new Bitmap(filename);
      BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, GLPixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
      bmp.UnlockBits(bmp_data);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    }
  }
}
