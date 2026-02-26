using Sober.Rendering.Shader;


namespace Sober.Rendering.HotReload
{
    public sealed class ShaderHotReloader : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly Queue<string> _dirty = new();
        private readonly Dictionary<string, List<ShaderProgram>> _map = new();
        private readonly string _watchedPath;
        private readonly string _relativeRoot;
        private DateTime _lastChange = DateTime.MinValue;
        private const int DebounceMs = 150;

        private static string FindProjectRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            for (int i = 0; i < 10 && dir != null; i++)
            {
                if (dir.GetFiles("*.csproj").Length > 0)
                    return dir.FullName;

                dir = dir.Parent;
            }

            throw new Exception("Could not locate project root (folder containing /Assets).");
        }

        private static readonly string ProjectRoot = FindProjectRoot();


        //remove noise files and temp files
        private static bool IsTempOrNoise(string path)
        {
            var name = Path.GetFileName(path);

            if (name.EndsWith("~")) return true;
            if (name.Contains(".TMP", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.StartsWith("~")) return true;

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext != ".frag" && ext != ".vert";
        }


        public ShaderHotReloader(string projectFolderRelative)
        {
            _relativeRoot = projectFolderRelative;
            var fullPath = Path.Combine(ProjectRoot, projectFolderRelative);
            if (!Directory.Exists(fullPath))
            {
                throw new Exception($"Project folder doesn't exist => {fullPath}");
            }

            _watchedPath = fullPath;  //assests

            _watcher = new FileSystemWatcher(fullPath)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
             _watcher.Renamed += OnRenamed;

            _watcher.EnableRaisingEvents = true;
        }


        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {

            var full = Path.GetFullPath(e.FullPath);
            if (IsTempOrNoise(full)) return;
            _dirty.Enqueue(full);
            _lastChange = DateTime.UtcNow;
        }


        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var full = Path.GetFullPath(e.FullPath);
            if (IsTempOrNoise(full)) return;
            _dirty.Enqueue(full);
            _lastChange = DateTime.UtcNow;
        }


        public void Track(ShaderProgram shader, params string[] files)
        {
            foreach (var file in files)
            {
                var key = Path.GetFullPath(Path.Combine(ProjectRoot, file)); // ABS KEY

                if (!_map.TryGetValue(key, out var list))
                {
                    list = new List<ShaderProgram>();
                    _map[key] = list;
                }

                if (!list.Contains(shader))
                    list.Add(shader);

                Console.WriteLine($"[HotReload] Tracking: {key}");
            }
        }

        public void PumpReloads()
        {
            if ((DateTime.UtcNow - _lastChange).TotalMilliseconds < DebounceMs)
                return;

            var unique = new HashSet<string>();
            while (_dirty.Count > 0) unique.Add(_dirty.Dequeue());

            foreach (var path in unique)
            {
                Console.WriteLine($"[HotReload] Changed: {path}");

                if (!_map.TryGetValue(path, out var shaders))
                {
                    Console.WriteLine($"[HotReload] Not tracked: {path}");
                    continue;
                }

                foreach (var shader in shaders)
                {
                    if (shader.TryReload(out var err))
                        Console.WriteLine($"[HotReload] OK: {path}");
                    else
                        Console.WriteLine($"[HotReload] Failed: {path} => {err}");
                }
            }
        }



        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
