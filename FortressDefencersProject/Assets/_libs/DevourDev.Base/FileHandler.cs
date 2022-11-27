using DevourDev.Base.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevourDev.Base
{
    public static class FileHandler
    {
        public static readonly char[] ForbittenFileNameChars = { (char)92, '@', '#', '%', '$', '^', '&', '*', '(', ')', '/', ':', '|', '\'', '"', '!' };

        public static bool WriteFile(string path, byte[] data)
        {
            try
            {
                using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
                fs.Write(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string FixFileName(string source, char replacer = '-')
        {
            return Hasher.ReplaceChars(source, ForbittenFileNameChars, replacer);
        }
        public static FileInfo GetLatestWritenFileFileInDirectory(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            FileInfo[] files = directoryInfo.GetFiles();
            DateTime lastWrite = DateTime.MinValue;
            FileInfo lastWritenFile = null;

            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastWrite)
                {
                    lastWrite = file.LastWriteTime;
                    lastWritenFile = file;
                }
            }
            return lastWritenFile;
        }

        public static void ExecuteProcess(string path)
        {
            Task.Run(() => ExecuteProcess_ext(path, null));
        }

        public static void ExecuteProcess(string path, string args)
        {
            Task.Run(() => ExecuteProcess_ext(path, args));
        }

        [DllImport("ProcessHandlerDll", EntryPoint = "ExecuteProcess", CharSet = CharSet.Ansi)]
        private static extern void ExecuteProcess_ext(string path, string args);



        [Obsolete("Use Process.Start(path, args) instead (not working in IL2CPP)")]
        public static Process ExecuteFile(string path, params string[] args)
        {
            StringBuilder sb = new();
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append(' ');
                string arg = args[i];
                if (arg.Contains(" "))
                {
                    sb.Append('"');
                    sb.Append(arg);
                    sb.Append('"');
                }
                else
                {
                    sb.Append(arg);
                }
            }

            Console.WriteLine(sb.ToString());
            return Process.Start(path, sb.ToString());
        }

        public static List<string> GetExecutionArguments(string prefix)
        {
            List<string> specialArgs = new();
            var allAgrs = Environment.GetCommandLineArgs();
            for (int i = 0; i < allAgrs.Length; i++)
            {
                if (allAgrs[i].StartsWith(prefix))
                {
                    specialArgs.Add(allAgrs[i]);
                }
            }

            return specialArgs;
        }

        public static Dictionary<string, string> GetDevourArgsAsDictionary(string keySign = "---", string valueSign = "===", string endValueSing = "-valend")
        {
            Dictionary<string, string> dic = new();

            var allCommandLineArgs = Environment.GetCommandLineArgs();

            UnityEngine.Debug.Log("all command line args (default):");
            foreach (var sa in allCommandLineArgs)
            {
                UnityEngine.Debug.Log(sa);
            };

            List<string> args = new();
            foreach (var a in allCommandLineArgs)
            {
                var words = a.Split(' ');
                foreach (var w in words)
                {
                    args.Add(w);
                }
            }

            UnityEngine.Debug.Log("re-splitted command line args (splitted):");
            foreach (var sa in args)
            {
                UnityEngine.Debug.Log(sa);
            };

            //List<string> devourArgs = new();

            //foreach (var a in allCommandLineArgs)
            //{
            //    if (a.StartsWith(keySign))
            //    {
            //        UnityEngine.Debug.Log("devour args detected! " + a);
            //        devourArgs.Add(a);
            //    }
            //    else
            //    {
            //        UnityEngine.Debug.Log("this is not devour args: " + a);
            //    }
            //}


            //if (devourArgs.Count == 0)
            //{
            //    UnityEngine.Debug.Log("no devour args found.");
            //    return dic;
            //}

            //string[] cmdlArgs;

            //if (devourArgs.Count > 1)
            //{
            //    StringBuilder singleCommangLineArgsStringBuilder = new();
            //    for (int i = 0; i < devourArgs.Count; i++)
            //    {
            //        if (i != 0)
            //            singleCommangLineArgsStringBuilder.Append(' ');
            //        singleCommangLineArgsStringBuilder.Append(devourArgs[i]);
            //    }

            //    cmdlArgs = singleCommangLineArgsStringBuilder.ToString().Split(' ');
            //}
            //else
            //{
            //    cmdlArgs = devourArgs[0].Split(' ');
            //}

            // UnityEngine.Debug.Log("total devour args found: " + cmdlArgs.Length);

            bool writingKey = false;
            bool writingValue = false;

            StringBuilder keyBuilder = new();
            StringBuilder valueBuilder = new();

            for (int i = 0; i < args.Count; i++)
            {
                var word = args[i];

                if (word == keySign)
                {
                    writingKey = true;
                    writingValue = false;
                    continue;
                }

                if (word == valueSign)
                {
                    writingKey = false;
                    writingValue = true;
                    continue;
                }

                if (word == endValueSing)
                {
                    writingKey = false;

                    if (writingValue)
                    {
                        writingValue = false;
                        string key = keyBuilder.ToString();
                        string value = valueBuilder.ToString();
                        if (!dic.TryAdd(key, value))
                        {
                            throw new ArgumentException($"ArgsParsing: Key {key} already exists in dictionary. Values: 0) {dic[key]}, 1) {value}");
                        }

                        keyBuilder.Clear();
                        valueBuilder.Clear();
                    }
                }

                if (writingKey)
                {
                    if (keyBuilder.Length != 0)
                        keyBuilder.Append(' ');

                    keyBuilder.Append(word);
                    continue;
                }

                if (writingValue)
                {
                    if (valueBuilder.Length != 0)
                        valueBuilder.Append(' ');

                    valueBuilder.Append(word);
                    continue;
                }
            }

            if (keyBuilder.Length > 0)
            {
                string lastKey = keyBuilder.ToString();

                if (valueBuilder.Length == 0)
                    throw new Exception($"ArgsParsing: last key {lastKey} has no value!");

                string lastValue = valueBuilder.ToString();

                if (!dic.TryAdd(lastKey, lastValue))
                {
                    throw new ArgumentException($"ArgsParsing: Key {lastKey} already exists in dictionary. Values: 0) {dic[lastKey]}, 1) {lastValue}");
                }
            }


            return dic;

        }

        public static List<(string, string)> GetArgsFromSingleString()
        {
            List<(string, string)> argsValues = new();

            var allCLArgs = Environment.GetCommandLineArgs();

            foreach (var a in allCLArgs)
            {
                DevourDev.MonoBase.UnityMainThreadDispatcher.InvokeOnMainThread(() => UnityEngine.Debug.LogError(a));
            }

            string allArgsString;

            if (allCLArgs.Length == 0)
                return argsValues;

            if (allCLArgs.Length != 1)
            {
                StringBuilder argsLine = new();

                for (int i = 0; i < allCLArgs.Length; i++)
                {
                    argsLine.Append(allCLArgs[i]);
                    if (i + 1 < allCLArgs.Length)
                    {
                        argsLine.Append(' ');
                    }
                }

                allArgsString = argsLine.ToString();
            }
            else
            {
                allArgsString = allCLArgs[0];
            }

            string[] words = allArgsString.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                argsValues.Add((words[i], words[++i]));
            }

            return argsValues;
        }

        [System.Obsolete("need to rework")]
        public static List<(string, string)> GetArgsValues(string prefix, string pointer)
        {
            // "--runmode = server" "--port = 7676", "--gardenKey = DFWwe2SDWsda" <- prefix == '--', pointer == '='

            List<(string, string)> argsValues = new();
            List<string> specialArgs = new();
            var allArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < allArgs.Length; i++)
            {
                if (allArgs[i].StartsWith(prefix))
                {
                    StringBuilder argNameBuilder = new();
                    StringBuilder argValueBuilder = new();
                    bool afterPointer = false;
                    var words = allArgs[i].Split(' ');
                    for (int j = 0; j < words.Length; j++)
                    {
                        string word = words[j];
                        if (word.StartsWith(prefix))
                        {
                            argNameBuilder.Append(word);

                            if (j < words.Length - 1 && words[j + 1] != pointer)
                                argNameBuilder.Append(' ');

                            continue;
                        }

                        if (word == pointer)
                        {
                            afterPointer = true;
                            continue;
                        }

                        if (afterPointer)
                        {
                            argValueBuilder.Append(word);

                            if (j < words.Length - 1)
                                argValueBuilder.Append(' ');

                            continue;
                        }
                    }

                    argsValues.Add((argNameBuilder.ToString(), argValueBuilder.ToString()));
                }
            }

            return argsValues;
        }
    }
}
