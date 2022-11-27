using FD.Networking.Garden;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace FD.Test
{
    public class CommandLineArgsPrinter : MonoBehaviour
    {
        [SerializeField] private string[] _pathToFile = { "cmd_line_args", "args.txt" };

        // Start is called before the first frame update
        private void Start()
        {
            var cmdArgs = System.Environment.GetCommandLineArgs();
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(_pathToFile[0]));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            using var fs = new FileStream(Path.Combine(dirPath, _pathToFile[1]), FileMode.Create, FileAccess.Write);

            for (int i = 0; i < cmdArgs.Length; i++)
            {
                if (i != 0)
                    fs.Write(System.Text.Encoding.UTF8.GetBytes(" "));

                string arg = cmdArgs[i];
                fs.Write(System.Text.Encoding.UTF8.GetBytes(arg));
            }
        }



    }
}
