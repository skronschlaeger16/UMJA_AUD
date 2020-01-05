using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMJA
{
    public class Compiler
    {
        public void ReadCsv(string path)
        {
            var filename = $"{path}\\filteredData.csv";

            using (var reader = new StreamReader(filename))
            {
                var line = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    string[] split = line.Split(';');

                    string folder = split[0];
                    string className = split[1];
                    string globals = split[2];
                    string methods = split[3];
                    bool getterSetter = bool.Parse(split[4].ToLower());
                    string classType = split[5];
                    List<string> getterNames = new List<string>();
                    List<string> setterNames = new List<string>();

                    Directory.CreateDirectory($"{path}\\{folder}");

                    using (var writer = new StreamWriter($"{path}\\{folder}\\{className}.java"))
                    {
                        #region ClassEnumsInterfaces
                        if (classType.Equals("Class"))
                        {
                            writer.WriteLine($"public class {className}");
                            writer.WriteLine($"{{");

                            #region GlobalVariables
                            var splitGlobals = globals.Split(',');

                            foreach (var item in splitGlobals)
                            {
                                string publicPrivate = "";
                                string zustand = "";
                                string variableName = "";
                                string variableType = "";
                                string value = "";

                                if (item.Contains("-"))
                                {
                                    publicPrivate = "private";
                                    item.Remove(0, 2);
                                }

                                else
                                {
                                    publicPrivate = "public";
                                    item.Remove(0, 2);
                                }

                                if (item.Contains("static"))
                                {
                                    zustand = "static";
                                    item.Remove(0, 6);
                                }

                                var splitColumn = item.Split(':');
                                variableName = splitColumn[0].Trim();

                                if (variableName.Contains("="))
                                {
                                    var splitName = variableName.Split('=');
                                    variableName = splitName[0].Trim();
                                    value = splitName[1].Trim();
                                }
                                
                                variableType = splitColumn[1].Trim();
                                var letters = variableName.ToCharArray();
                                letters[0] = char.ToUpper(letters[0]);
                                getterNames.Add($"{variableType} get{new string(letters)}");
                                setterNames.Add($"void set{new string(letters)} ({variableType} set)");

                                if (Char.IsUpper(variableName[0]))
                                {
                                    if (zustand.Contains("static"))
                                    {
                                        zustand = zustand + " final";
                                    }

                                    else
                                    {
                                        zustand = "final";
                                    }
                                }

                                if (value.Equals(""))
                                {
                                    writer.WriteLine($"{publicPrivate} {zustand} {variableType} {variableName};");
                                }

                                else
                                {
                                    writer.WriteLine($"{publicPrivate} {zustand} {variableType} {variableName} = {value};");
                                }
                            }
                            #endregion

                            #region GetterSetter
                            if (getterSetter == true)
                            {
                                foreach (var item in getterNames)
                                {
                                    writer.WriteLine($"public {item} (){{");
                                    var splitGetter = item.Split(' ');
                                    var letters = splitGetter[1].Remove(0, 2).ToCharArray();
                                    letters[0] = char.ToLower(letters[0]);

                                    writer.WriteLine($"return {new string(letters)};");
                                    writer.WriteLine($"}}");
                                }

                                foreach(var item in setterNames)
                                {
                                    writer.WriteLine($"{item} {{");
                                    var splitSetter = item.Split(' ');
                                    var letters = splitSetter[1].Remove(0, 2).ToCharArray();
                                    letters[0] = char.ToLower(letters[0]);

                                    writer.WriteLine($"{new string(letters)} = set;");
                                    writer.WriteLine($"}}");
                                }
                            }
                            #endregion

                            #region Methods
                            var splitMethods = methods.Split(',');

                            foreach (var item in splitMethods)
                            {
                                string methodLine = item;
                                string publicPrivate = "";
                                string zustand = "";
                                string methodType = "";
                                string methodnameAndParameters = "";
                                string methodName = "";
                                string parameters = "";
                                List<string> parameterList = new List<string>();

                                if (methodLine.Contains("-"))
                                {
                                    publicPrivate = "private";
                                    methodLine = methodLine.Remove(0, 2);
                                }

                                else
                                {
                                    publicPrivate = "public";
                                    methodLine = methodLine.Remove(0, 2);
                                }

                                if (methodLine.Contains("static"))
                                {
                                    zustand = "static";
                                    methodLine = methodLine.Remove(0, 7);
                                }

                                if (!methodLine.EndsWith(")"))
                                {
                                    int idx = methodLine.LastIndexOf(':');

                                    if(idx != -1)
                                    {
                                        methodnameAndParameters = methodLine.Substring(0, idx).Trim();
                                        methodType = methodLine.Substring(idx + 1).Trim();
                                    }

                                    methodnameAndParameters = methodnameAndParameters.Replace('(', ' ');
                                    methodnameAndParameters = methodnameAndParameters.Remove(methodnameAndParameters.Length - 1);

                                    var splitMethodFromParameters = methodnameAndParameters.Split(' ');
                                    methodName = splitMethodFromParameters[0];
                                    var splitParameters = splitMethodFromParameters[1].Split(',');

                                    foreach (var parameter in splitParameters)
                                    {
                                        var splitColumn = parameter.Split(':');
                                        string variableName = splitColumn[0].Trim();
                                        string variableType = splitColumn[1].Trim();

                                        if (parameters.Equals(""))
                                        {
                                            parameters = $"{variableType} {variableName}";
                                        }

                                        else
                                        {
                                            parameters = $"{parameters}, {variableType} {variableName}";
                                        }
                                    }

                                    if (Char.IsUpper(methodName[0]))
                                    {
                                        if (zustand.Equals(""))
                                        {
                                            zustand = "final";
                                        }

                                        else
                                        {
                                            zustand = $"{zustand} final";
                                        }
                                    }
                                    writer.WriteLine($"{publicPrivate} {zustand} {methodType} {methodName} ({parameters}) {{");
                                    writer.WriteLine();
                                    writer.WriteLine($"}}");
                                }

                                else
                                {
                                    if(item.ElementAt(item.IndexOf("(") + 1).Equals(")"))
                                    {
                                        item.Replace('(', ' ');
                                        item.Remove(item.Length - 1);

                                        writer.WriteLine($"{publicPrivate} {item} () {{");
                                        writer.WriteLine();
                                        writer.WriteLine($"}}");
                                    }

                                    else
                                    {
                                        item.Replace('(', ' ');
                                        item.Remove(item.Length - 1);

                                        var splitConstructor = item.Split(' ');
                                        methodName = splitConstructor[0];
                                        var splitParameters = splitConstructor[1].Split(',');

                                        foreach (var parameter in splitParameters)
                                        {
                                            var splitColumn = parameter.Split(':');
                                            string variableName = splitColumn[0].Trim();
                                            string variableType = splitColumn[1].Trim();

                                            if (parameters.Equals(""))
                                            {
                                                parameters = $"{variableType} {variableName}";
                                                parameterList.Add(variableName);
                                            }

                                            else
                                            {
                                                parameters = $"{parameters}, {variableType} {variableName}";
                                                parameterList.Add(variableName);
                                            }
                                        }

                                        writer.WriteLine($"{publicPrivate} {methodName} ({parameters}) {{");

                                        foreach (var paramItem in parameterList)
                                        {
                                            writer.WriteLine($"this.{paramItem} = {paramItem};");
                                        }

                                        writer.WriteLine($"}}");
                                    }
                                }
                            }
                            #endregion
                        }

                        else if (classType.Equals("Enumeration"))
                        {
                            writer.WriteLine($"public enum {className}");
                            writer.WriteLine($"{{");

                            #region Enums
                            string enumStr = "";
                            var splitGlobals = globals.Split(',');

                            foreach (var item in splitGlobals)
                            {
                                if (enumStr.Length.Equals(0))
                                {
                                    enumStr = $"{item}";
                                }

                                else
                                {
                                    enumStr = $"{enumStr}, {item}";
                                }
                            }

                            writer.WriteLine($"{enumStr}");
                            #endregion
                        }

                        else if (classType.Equals("Interface"))
                        {
                            writer.WriteLine($"public interface {className}");
                            writer.WriteLine($"{{");

                            #region InterfaceVariables
                            var splitGlobals = globals.Split(',');

                            foreach (var item in splitGlobals)
                            {
                                string publicPrivate = "";
                                string zustand = "";
                                string variableName = "";
                                string variableType = "";
                                string value = "";

                                if (item.Contains("-"))
                                {
                                    publicPrivate = "private";
                                    item.Remove(0, 2);
                                }

                                else
                                {
                                    publicPrivate = "public";
                                    item.Remove(0, 2);
                                }

                                if (item.Contains("static"))
                                {
                                    zustand = "static";
                                    item.Remove(0, 6);
                                }

                                var splitColumn = item.Split(':');
                                variableName = splitColumn[0].Trim();

                                if (variableName.Contains("="))
                                {
                                    var splitName = variableName.Split('=');
                                    variableName = splitName[0].Trim();
                                    value = splitName[1].Trim();
                                }

                                variableType = splitColumn[1].Trim();
                                var letters = variableName.ToCharArray();
                                letters[0] = char.ToUpper(letters[0]);
                                getterNames.Add($"{variableType} get{new string(letters)}");
                                setterNames.Add($"void set{new string(letters)}");

                                if (Char.IsUpper(variableName[0]))
                                {
                                    if (zustand.Contains("static"))
                                    {
                                        zustand = zustand + " final";
                                    }

                                    else
                                    {
                                        zustand = "final";
                                    }
                                }

                                if (value.Equals(""))
                                {
                                    writer.WriteLine($"{publicPrivate} {zustand} {variableType} {variableName};");
                                }

                                else
                                {
                                    writer.WriteLine($"{publicPrivate} {zustand} {variableType} {variableName} = {value};");
                                }
                            }
                            #endregion

                            #region InterfaceMethods
                            var splitMethods = methods.Split(',');

                            foreach (var item in splitMethods)
                            {
                                string publicPrivate = "";
                                string zustand = "";
                                string methodType = "";
                                string methodnameAndParameters = "";
                                string methodName = "";
                                string parameters = "";
                                List<string> parameterList = new List<string>();

                                if (item.Contains("-"))
                                {
                                    publicPrivate = "private";
                                    item.Remove(0, 1);
                                }

                                else
                                {
                                    publicPrivate = "public";
                                    item.Remove(0, 1);
                                }

                                if (item.Contains("static"))
                                {
                                    zustand = "static";
                                    item.Remove(0, 6);
                                }

                                if (!item.EndsWith(")"))
                                {
                                    int idx = item.LastIndexOf(':');

                                    if (idx != -1)
                                    {
                                        methodnameAndParameters = item.Substring(0, idx).Trim();
                                        methodType = item.Substring(idx + 1).Trim();
                                    }

                                    methodnameAndParameters.Replace('(', ' ');
                                    methodnameAndParameters.Remove(methodnameAndParameters.Length - 1);

                                    var splitMethodFromParameters = methodnameAndParameters.Split(' ');
                                    methodName = splitMethodFromParameters[0];
                                    var splitParameters = splitMethodFromParameters[1].Split(',');

                                    foreach (var parameter in splitParameters)
                                    {
                                        var splitColumn = parameter.Split(':');
                                        string variableName = splitColumn[0].Trim();
                                        string variableType = splitColumn[1].Trim();

                                        if (parameters.Equals(""))
                                        {
                                            parameters = $"{variableType} {variableName}";
                                        }

                                        else
                                        {
                                            parameters = $"{parameters}, {variableType} {variableName}";
                                        }
                                    }

                                    if (Char.IsUpper(methodName[0]))
                                    {
                                        if (zustand.Equals(""))
                                        {
                                            zustand = "final";
                                        }

                                        else
                                        {
                                            zustand = $"{zustand} final";
                                        }
                                    }

                                    writer.WriteLine($"{publicPrivate} {zustand} {methodType} {methodName} ({parameters});");
                                }
                            }
                            #endregion
                        }
                        #endregion

                        writer.WriteLine($"}}");
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
        }
    }
}
