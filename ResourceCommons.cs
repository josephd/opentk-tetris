//|-----------------------------------------------------------------------|\\
//| ResourceCommons.cs                                                    |\\
//| Stores resources common to all components                             |\\
//|                                                                       |\\
//| Joseph Dillon                                                         |\\
//|-----------------------------------------------------------------------|\\
//| opentk-tetris, a tetris game                                          |\\
//| Copyright (C) 2012  Joseph Dillon                                     |\\
//|                                                                       |\\
//| This program is free software: you can redistribute it and/or modify  |\\
//| it under the terms of the GNU General Public License as published by  |\\
//| the Free Software Foundation, either version 3 of the License, or     |\\
//| (at your option) any later version.                                   |\\
//|                                                                       |\\
//| This program is distributed in the hope that it will be useful,       |\\
//| but WITHOUT ANY WARRANTY; without even the implied warranty of        |\\
//| MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         |\\
//| GNU General Public License for more details.                          |\\
//|                                                                       |\\
//| You should have received a copy of the GNU General Public License     |\\
//| along with this program.  If not, see <http://www.gnu.org/licenses/>. |\\
//|-----------------------------------------------------------------------|\\

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
using QuickFont;

using Texture = System.Int32;

using GLPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Tetris
{
  static class ResourceCommons
  {
    public const string RESOURCE_DIR = "assets";
    public const string TEXTURES_DIR = "textures";
    public const string MODELS_DIR   = "models";
    public const string SHADERS_DIR  = "shaders";
    public const string FONTS_DIR    = "fonts";
    public const string START_DIR    = ".";

    public static int TetrionTexture;
    public static int Block;
    public static int BlockGhost;
    public static int TetrionTextureSampler;
    public static int BlockSampler;
    public static int BlockGhostSampler;
    public static Dictionary<TetraminoColor, MeshRenderer> Blocks             = new Dictionary<TetraminoColor, MeshRenderer>();
    public static Dictionary<TetraminoType, Bitmap>        BlockOverlays      = new Dictionary<TetraminoType, Bitmap>();
    public static Dictionary<TetraminoType, Bitmap>        BlockOverlaysSmall = new Dictionary<TetraminoType, Bitmap>();
    public static MeshRenderer Tetrion;
    public static MeshRenderer Panel;
    public static int Simple_Shader;
    public static int LightPositionUniform;
    public static int LightDiffuseUniform;
    public static int SamplerUniform;
    public static Bitmap PanelBase;
    public static QFont LiberationSans;

    static int Simple_vs;
    static int Simple_fs;

    static XmlSerializer modelSerializer = new XmlSerializer(typeof(Mesh));

    public static void Load()
    {
      BlockOverlays.Add(TetraminoType.I, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "I.png")));
      BlockOverlays.Add(TetraminoType.O, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "O.png")));
      BlockOverlays.Add(TetraminoType.J, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "J.png")));
      BlockOverlays.Add(TetraminoType.L, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "L.png")));
      BlockOverlays.Add(TetraminoType.S, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "S.png")));
      BlockOverlays.Add(TetraminoType.Z, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Z.png")));
      BlockOverlays.Add(TetraminoType.T, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "T.png")));
      BlockOverlaysSmall.Add(TetraminoType.I,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Is.png")));
      BlockOverlaysSmall.Add(TetraminoType.O,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Os.png")));
      BlockOverlaysSmall.Add(TetraminoType.J,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Js.png")));
      BlockOverlaysSmall.Add(TetraminoType.L,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Ls.png")));
      BlockOverlaysSmall.Add(TetraminoType.S,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Ss.png")));
      BlockOverlaysSmall.Add(TetraminoType.Z,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Zs.png")));
      BlockOverlaysSmall.Add(TetraminoType.T,    new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Ts.png")));
      BlockOverlaysSmall.Add(TetraminoType.Null, new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "Ns.png")));
      LoadTexture(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "tetrion.png"), out TetrionTexture, out TetrionTextureSampler);
      LoadTexture(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "block.png"), out Block, out BlockSampler);
      LoadTexture(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "blockGhost.png"), out BlockGhost, out BlockGhostSampler);
      PanelBase = new Bitmap(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), TEXTURES_DIR), "panel.png"));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockCyan.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Cyan, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockYellow.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Yellow, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockPurple.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Purple, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockGreen.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Green, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockRed.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Red, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockBlue.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Blue, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "blockOrange.xml"), FileMode.Open))
	Blocks.Add(TetraminoColor.Orange, new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp)));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "tetrion.xml"), FileMode.Open))
	Tetrion = new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp));
      using (Stream tmp = File.Open(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), MODELS_DIR), "panel.xml"), FileMode.Open))
	Panel = new MeshRenderer((Mesh)modelSerializer.Deserialize(tmp));
      using (StreamReader vs = new StreamReader(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), SHADERS_DIR), "vs_simple.glsl")))
	using (StreamReader fs = new StreamReader(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), SHADERS_DIR), "fs_simple.glsl")))
	  LoadShader(vs.ReadToEnd(), fs.ReadToEnd(), out Simple_vs, out Simple_fs, out Simple_Shader);
      LightPositionUniform = GL.GetUniformLocation(Simple_Shader, "lightPosition");
      LightDiffuseUniform = GL.GetUniformLocation(Simple_Shader, "lightDiffuse");
      SamplerUniform = GL.GetUniformLocation(Simple_Shader, "color_texture");
      LiberationSans = new QFont(Path.Combine(Path.Combine(Path.Combine(START_DIR, RESOURCE_DIR), FONTS_DIR), "LiberationSans.ttf"), 64);
      LiberationSans.Options.Colour = new Color4(1.0f, 0.0f, 0.0f, 1.0f);
    }

    public static void Unload()
    {
      GL.DeleteTextures(1, ref TetrionTexture);
      GL.DeleteTextures(1, ref Block);
      foreach (KeyValuePair<TetraminoColor, MeshRenderer> block in Blocks)
	block.Value.Free();
      Panel.Free();
      Tetrion.Free(); // RELEASE THE TETRION!! (sorry, no kraken)
      GL.DeleteShader(Simple_vs);
      GL.DeleteShader(Simple_fs);
      GL.DeleteProgram(Simple_Shader);
    }

    public static void LoadShader(string vs, string fs, out int vertexObject, out int fragmentObject, out int program)
    {
      int status_code;
      string info;
      vertexObject = GL.CreateShader(ShaderType.VertexShader);
      fragmentObject = GL.CreateShader(ShaderType.FragmentShader);
      GL.ShaderSource(vertexObject, vs);
      GL.CompileShader(vertexObject);
      GL.GetShaderInfoLog(vertexObject, out info);
      GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out status_code);
      if (status_code != 1)
	throw new ApplicationException(info);
      GL.ShaderSource(fragmentObject, fs);
      GL.CompileShader(fragmentObject);
      GL.GetShaderInfoLog(fragmentObject, out info);
      GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out status_code);
      if (status_code != 1)
	throw new ApplicationException(info);
      program = GL.CreateProgram();
      GL.AttachShader(program, fragmentObject);
      GL.AttachShader(program, vertexObject);
      GL.LinkProgram(program);
    }

    public static void LoadTexture(string filename, out Texture id, out int samplerId)
    {
      if (String.IsNullOrEmpty(filename))
        throw new ArgumentException(filename);
      LoadTexture(new Bitmap(filename), out id, out samplerId);
    }

    public static void LoadTexture(Bitmap bmp, out Texture id, out int samplerId)
    {
      // Store Texture ID
      id = GL.GenTexture();
      // Bind Texture
      GL.BindTexture(TextureTarget.Texture2D, id);
      // Load Bitmap
      BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, GLPixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
      bmp.UnlockBits(bmp_data);
      // Create a Sampler for the Texture
      GL.GenSamplers(1, out samplerId);
      // Set the texture access and bondries information for the Sampler
      GL.SamplerParameter(samplerId, SamplerParameter.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.SamplerParameter(samplerId, SamplerParameter.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.SamplerParameter(samplerId, SamplerParameter.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.SamplerParameter(samplerId, SamplerParameter.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
    }

    public static void UpdateTexture(Bitmap bmp, Texture id, int x, int y, int w, int h)
    {
      GL.BindTexture(TextureTarget.Texture2D, id);
      BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, y, w, h, GLPixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
      bmp.UnlockBits(bmp_data);
    }
  }

  class ResourceCommonsLoader : GameComponent
  {
    static bool loaded = false;

    public ResourceCommonsLoader(GameWindow window)
      : base(window)
    {
      
    }

    protected override void DoUnLoad()
    {
      if (loaded)
	ResourceCommons.Unload();
    }

    protected override void DoLoad()
    {
      if (!loaded)
	ResourceCommons.Load();
    }

    protected override void DoUpdate(FrameEventArgs e) { }

    protected override void DoDraw(FrameEventArgs e) { }
  }
}
