using System.IO;
using UnityEngine;

namespace FD.Networking.Garden
{
    public class ApplicationLauncherDefault : ApplicationLauncherBase
    {
        [SerializeField, Tooltip("relative")] private string[] _applicationPath = { "Realm", "FortressDefencers.exe" };

        [SerializeField] private bool _useAbsolutePath = false;
        [SerializeField] private string _absPath = @"D:\UNITY_PROJECTS\FortressDefencersProject\Builds\localhost\GARDEN_SERVER\Realm\FortressDefencers.exe";

        private void Start()
        {
#if UNITY_EDITOR
            _useAbsolutePath = true;
#endif
        }


        public override void LaunchApp(string args)
        {
            //path = Directory.GetCurrentDirectory() + "Realm\\FortressDefencers.exe";
            //Debug.Log($"APPLICATION_LAUNCHER_DEFAULT:: launching path: {path}, args: {args}");
            //DevourDev.Base.FileHandler.ExecuteProcess(path, args);
            //return;
            string p = _useAbsolutePath ? _absPath : GetExecutableRootPath();
            Debug.Log($"APPLICATION_LAUNCHER_DEFAULT:: launching path: {p}, args: {args}");
            DevourDev.Base.FileHandler.ExecuteProcess(p, args);
        }

        public string GetExecutablePath()
        {
            return Path.Combine(_applicationPath);
        }
        public string GetExecutableRootPath()
        {
            var dir = Directory.GetCurrentDirectory();
            string[] p = new string[_applicationPath.Length + 1];
            p[0] = dir;
            for (int i = 1; i <= _applicationPath.Length; i++)
            {
                p[i] = _applicationPath[i - 1];
            }
            return Path.Combine(p);
        }
        public string GetRootPathForNewFile(string newFileName)
        {
            var dir = Directory.GetCurrentDirectory();
            string[] p = new string[_applicationPath.Length + 1];
            p[0] = dir;
            for (int i = 1; i <= _applicationPath.Length; i++)
            {
                p[i] = _applicationPath[i - 1];
            }
            p[^1] = newFileName;
            return Path.Combine(p);
        }
    }
}
