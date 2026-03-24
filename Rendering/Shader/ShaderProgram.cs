using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Sober.Rendering.Shader
{
    public class ShaderProgram : IDisposable
    {
        public int Test_Data { get; private set; }


        //store color and positions of shaders
        private readonly Dictionary<string, int> _uniformLocations;

        
        // hot reloader
        private readonly string _vertexPath;
        private readonly string _fragmentPath;


        public ShaderProgram(string vertexPath, string fragmentPath)
        {
            _vertexPath= vertexPath;
            _fragmentPath = fragmentPath;

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

            // cache 
            _uniformLocations = new Dictionary<string, int>();
            CacheUniforms();
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



        //hot reloader
        public bool TryReload(out string error)
        {
            error = "";
            try
            {
                string newVert = ReadShaderText(_vertexPath);
                string newFrag= ReadShaderText(_fragmentPath);

                int newProgram = BuildProgram(newVert, newFrag);

                GL.DeleteProgram(Test_Data);
                Test_Data = newProgram;

                CacheUniforms();
                Bind();
                if (_uniformLocations.TryGetValue("u_Scene", out int loc) && loc != -1)
                    GL.Uniform1(loc, 0);
                UnBind();

                return true;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }        

        private static string FindProjectRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            for (int i = 0; i < 12 && dir != null; i++)
            {
                if (dir.GetFiles("*.csproj").Length > 0)
                    return dir.FullName;
                dir = dir.Parent;
            }
            throw new Exception("Could not locate project root (folder containing *.csproj).");
        }

        private static readonly string ProjectRoot = FindProjectRoot();
        private string ReadShaderText(string path)
        {
            var path_Combined = Path.Combine(ProjectRoot, path);
            if (File.Exists(path_Combined))
            {
                return File.ReadAllText(path_Combined);
            }
            var binPath = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(binPath))
            {
                return File.ReadAllText(binPath);
            }
            throw new Exception($"Shader file not found:\n- {path_Combined}\n- {binPath}");

        }

        private int  BuildProgram(string vertSrc, string fragSrc)
        {
            var vertexShader = CompileShader(ShaderType.VertexShader, vertSrc, _vertexPath);
            var fragmentShader = CompileShader(ShaderType.FragmentShader, fragSrc, _fragmentPath);
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if(status == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"Program Linking Faild {info}");
            }
            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }

        private void CacheUniforms()
        {
            _uniformLocations.Clear();
            GL.GetProgram(Test_Data, GetProgramParameterName.ActiveUniforms, out int count);
            for(int i=0; i< count; i++)
            {
                string name = GL.GetActiveUniform(Test_Data, i, out _, out _);
                int location = GL.GetUniformLocation(Test_Data, name);
                _uniformLocations[name] = location;
            }
        }


        private readonly HashSet<string> _missing = new();
        public void SetInt(string name, int value)
        {
            if (!_uniformLocations.TryGetValue(name, out int loc) || loc == -1)
            {
                if (_missing.Add(name))
                    Console.WriteLine($"Warning: Uniform '{name}' not found in shader.");
                return;
            }
            GL.Uniform1(loc, value);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Test_Data);
        }
    }
}
