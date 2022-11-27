using System;
using System.Collections.Generic;
using System.Text;


namespace DevourDev.ConsoleInputs
{
    public class UserInputInterpolation
    {
        //todo: do =)
        private string _interpolationCommand;
        private Dictionary<string, Func<string>> _varDic;


        public UserInputInterpolation(string interpolationCommand)
        {
            _varDic = new();
            _interpolationCommand = interpolationCommand;
        }


        public bool TryAddVarialbe(string command, Func<string> getter) => _varDic.TryAdd(command, getter);


        public string Interpolate(string source)
        {
            var mainSB = new StringBuilder(source.Length);
            var variableBuilder = new StringBuilder();

            bool variableBuilding = false;
            var sourceLen = source.Length;
            for (int i = 0; i < sourceLen; i++)
            {
                char c = source[i];

                if (c == _interpolationCommand[0])
                {
                    for (int j = 1; j < _interpolationCommand.Length; j++)
                    {
                        int index = i + j;
                        if (index >= sourceLen)
                            goto MainTexting;

                        if (source[index] != _interpolationCommand[j])
                            goto MainTexting;
                    }

                    i += _interpolationCommand.Length - 1;
                    if (variableBuilding)
                    {
                        string v = variableBuilder.ToString();
                        if (_varDic.TryGetValue(v, out var f))
                            mainSB.Append(f());
                        else
                            mainSB.Append($"{_interpolationCommand}{v}{_interpolationCommand}");

                        variableBuilder.Clear();
                    }
                    variableBuilding = !variableBuilding;
                    continue;
                }

                if (variableBuilding)
                {
                    variableBuilder.Append(c);
                    continue;
                }


            MainTexting:

                mainSB.Append(c);
            }

            return mainSB.ToString();
        }
    }
}
