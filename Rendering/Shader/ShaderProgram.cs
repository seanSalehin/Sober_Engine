using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sober.Rendering.Shader
{
    public class ShaderProgram : IDisposable
    {
        public int Test_Data { get; private set; }


        //store color and positions of shaders
        private readonly Dictionary<string, int> _uniformLocations;



        public ShaderProgram(string vertexPath, string fragmentPath)
        {

            //load vertix and fragment
            string vertexFullPath = Path.Combine(AppContext.BaseDirectory, vertexPath);
            string fragmentFullPath = Path.Combine(AppContext.BaseDirectory, fragmentPath);

            if (!File.Exists(vertexFullPath))
                throw new Exception("Vertex shader not found: " + vertexFullPath);

            if (!File.Exists(fragmentFullPath))
                throw new Exception("Fragment shader not found: " + fragmentFullPath);

            string vertexSource = File.ReadAllText(vertexFullPath);
            string fragmentSource = File.ReadAllText(fragmentFullPath);



            //compile
            int vertixShader = CompileShader(ShaderType.VertexShader, vertexSource, vertexPath);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource, fragmentPath);


            //testing shaders
            //Add data to shaders
            Test_Data = GL.CreateProgram();
            GL.AttachShader(Test_Data, vertixShader);
            GL.AttachShader(Test_Data, fragmentShader);
            GL.LinkProgram(Test_Data);

            // checking status
            GL.GetProgram(Test_Data, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string info = GL.GetProgramInfoLog(Test_Data);
                throw new Exception($"Program Linking Faild {info}");
            }

            //remove data and deleting shaders
            GL.DetachShader(Test_Data, vertixShader);
            GL.DetachShader(Test_Data, fragmentShader);
            GL.DeleteShader(vertixShader);
            GL.DeleteShader(fragmentShader);

            // cache (instead of GL.GetUniformLocation) 
            _uniformLocations = new Dictionary<string, int>();
            GL.GetProgram(Test_Data, GetProgramParameterName.ActiveUniforms, out int count);
            for (int i = 0; i < count; i++)
            {
                string name = GL.GetActiveUniform(Test_Data, i, out _, out _);
                _uniformLocations[name] = GL.GetUniformLocation(Test_Data, name);
            }
        }


        //compiler
        private int CompileShader(ShaderType type, string source, string path)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL. CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($" {type} compilation failed :{info}");
            }
            return shader;
        }


        //control shader
        //use shader for drawing
        public void Bind() => GL.UseProgram(Test_Data);

        //stop using shader
        public void UnBind() => GL.UseProgram(0);

        //send a float to shader
        public void SetFloat(string name, float value)
        {
            int location = GL.GetUniformLocation(Test_Data, name);
            if (location == -1)
            {
                Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
                return;
            }
            GL.Uniform1(location, value);
        }

        //send a color (4d vector) to shader
        public void SetVector4(string name, Vector4 value) => GL.Uniform4(_uniformLocations[name], value);

        //send a 4*4 matrix (transform) to the shader
        public void SetMatrix4(string name, Matrix4 value) => GL.UniformMatrix4(_uniformLocations[name], false, ref value);


        //clean up to free up graphics memory
        public void Dispose()
        {
            GL.DeleteProgram(Test_Data);
        }
    }
}
