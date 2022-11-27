using UnityEngine;

namespace FD.Networking.Garden
{
    public abstract class ApplicationLauncherBase : MonoBehaviour
    {
        public abstract void LaunchApp(string args);
    }
}
