using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Umja
{
    public class Parser
    {
        Collection<string> built_Line = new Collection<string>();
        #region Parse ---> Reads the File and looks which Method should be called for the individual line.
        public void Parse(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                var line = reader.ReadLine();

                while (line != null)
                {
                    if (line.Contains("html") && line.Contains("&") || line.StartsWith("- &") || line.StartsWith("+ &"))
                    {
                        built_Line.Add(Special_Sign_Lines_Get_Added(line));
                    }
                    else if (line.Contains("y:NodeLabel"))
                    {
                        built_Line.Add(Class_Or_Package_Found(line));
                    }
                    else if (line.Contains("y:AttributeLabel"))
                    {
                        built_Line.Add(Finding_And_Deleting_Labels(line, "y:AttributeLabel"));
                    }
                    else if (line.Contains(" : ") || line.Contains("(") && !line.StartsWith("(") || line.Contains("Getter/Setter"))
                    {
                        built_Line.Add(Finding_And_Deleting_Labels(line, "y:MethodLabel"));
                    }
                    else if (line.Contains("stereotype"))
                    {
                        if (line.Contains("enumeration")) built_Line.Add("Enumeration");
                        else if (line.Contains("interface")) built_Line.Add("Interface");
                        else if (line.Contains("abstrakt")) built_Line.Add("Abstrakt");
                        else if (line.Contains("")) built_Line.Add("Class");
                    }
                    line = reader.ReadLine();
                }
            }
            Write_To_CSV();

        }

        #endregion

        #region Write_To_CSV ----> Writes all Data of the .graphml into an CSV Data

        private void Write_To_CSV(string LukesParth)
        {
            
            var path = "UmjaHoffentlichFunktioniertsJetzt\\UMJA\\UMJA\\";
            if (LukesParth!=null) path = LukesParth;
            var package = "";
            var csv_Line = "";
            var class_Type = "";
            var has_Getter_Setter = false;
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Package;Class;Global_Variables(splitted by ,);Methods(Parameters)(splitted by , );Getter/Setter(true or false);Class Type");
                foreach (var item in built_Line)
                {
                    if (!item.Equals("") && !item.Contains("Class A") && !item.Contains("Class B") && !item.Equals("Interface I") && !item.Contains("Comment"))
                    {

                        if (item.Contains(".") && !item.Equals(csv_Line))
                        {
                            package = $"{item};";
                        }
                        else if (!item.Contains(":") && !item.Contains(",") && !item.Contains(".") && !item.Contains("+") && !item.Contains("-") && !item.Contains("/") && !item.Contains("(") && !item.Equals("Class") && !item.Equals("Enumeration") && !item.Equals("Interface") && !item.Equals("Abstrakt"))
                        {
                            New_Class_And_New_Line(package, ref csv_Line, ref has_Getter_Setter, writer, item, class_Type);
                        }
                        else if (item.Contains(" : ") || item.Contains("+") || item.Contains("-") || item.Contains("("))
                        {
                            csv_Line = Attribute_Or_Method(csv_Line, item);
                        }
                        else if (item.Contains("Getter")) has_Getter_Setter = true;
                        else if (item.Contains("Enum") || item.Contains("Interface") || item.Contains("Abstrakt") || item.Contains("Class")) class_Type = item;
                        else if (item.Contains(",")) csv_Line = csv_Line + item;
                    }
                }
                csv_Line = csv_Line + $";{has_Getter_Setter}";
                csv_Line = csv_Line + $";{class_Type}";
                writer.Write(csv_Line);
                writer.Flush();
                writer.Close();
            }
        }
        #endregion

        #region Attribute_Or_Method ----> Detects if it is an Method or Attribute and adds them

        private static string Attribute_Or_Method(string csv_Line, string item)
        {
            if (item.Contains(" : ") && !item.Contains("("))
            {
                if (!csv_Line.EndsWith(";") && !csv_Line.EndsWith(",")) csv_Line = csv_Line + $",{item}";
                else if (!csv_Line.EndsWith(";") && !csv_Line.EndsWith(",")) csv_Line = csv_Line + $";{item},";
                else csv_Line = csv_Line + $" {item},";
            }
            else if (item.Contains(" : ") && item.Contains("(") || item.Contains("()"))
            {
                if (csv_Line.EndsWith(",") && !csv_Line.Contains("("))
                {
                    csv_Line = csv_Line.TrimEnd(',');
                    csv_Line = csv_Line + $";{item}";
                }
                else csv_Line = csv_Line + $",{item}";
            }
            return csv_Line;
        }

        #endregion
        #region New_Class_And_New_Line ---> when a new Class got found, then the line gets written and the line will be reset to "package;newClass"

        private static void New_Class_And_New_Line(string package, ref string csv_Line, ref bool has_Getter_Setter, StreamWriter writer, string item, string class_Type)
        {
            if (!csv_Line.Equals(""))
            {
                csv_Line = csv_Line + $";{has_Getter_Setter}";
                csv_Line = csv_Line + $";{class_Type}";
                writer.WriteLine(csv_Line);
            }
            csv_Line = package + $"{item};";
            has_Getter_Setter = false;
        }

        #endregion
        #region Special_Sign_Lines_Get_Added ----> Looks for Lines with Special Signs like "html", "&lt;", usw. and discards these parts.

        private string Special_Sign_Lines_Get_Added(string line)
        {
            var throw_Away_Every_Special_Tag_With_HTML = line.Split('&');
            var public_Or_Private = true;
            var is_Static = false;
            foreach (var item in throw_Away_Every_Special_Tag_With_HTML)
            {
                if (item.Contains("lt;u")) is_Static = true;
                if (!item.Contains("&") && !item.Contains("html") && !item.Contains("+") && !item.Contains("-") && !item.Contains("lt") && item.Contains(" : "))
                    if (public_Or_Private)
                    {
                        var public_private_Item = Public_Private_Symbol(item, "+", is_Static);
                        return public_private_Item;
                    }
                    else
                    {
                        var public_Private_Item = Public_Private_Symbol(item, "-", is_Static);
                        return public_Private_Item;
                    }
                if (item.Contains("-")) public_Or_Private = false;
            }
            return "";
        }
        #endregion
        #region Public_Private_Symbol ----> Adds the Plus or Minus if the Method is Public or Private

        private string Public_Private_Symbol(string item, string symbol, bool is_Static)
        {

            var public_Item = item.Replace("gt;", "");
            if (is_Static) public_Item = $"{symbol} static {public_Item}";
            else public_Item = symbol + public_Item;

            return public_Item;
        }

        #endregion

        #region Finding_And_Deleting_Labels -----> This Method Deletes Every Label in the Line and deletes every empty Space at the beginning
        private string Finding_And_Deleting_Labels(string line, string label_Denotation)
        {
            var deletedLabel = line.Replace($"<{label_Denotation}>", "");
            deletedLabel = deletedLabel.Replace($"</{label_Denotation}>", "");
            deletedLabel = deletedLabel.Replace($"<{label_Denotation}/>", "");
            deletedLabel = deletedLabel.Replace("ggf.", "");
            while (deletedLabel.StartsWith(" ")) deletedLabel = deletedLabel.Substring(1);
            return deletedLabel;
        }
        #endregion

        #region Class_Or_Package_Found ------> Classes and Packages will be searched and picked out of the Label
        private string Class_Or_Package_Found(string line)
        {

            var class_Name = line.Split('>');
            foreach (var item in class_Name)
            {
                if (item.Contains("LabelModel"))
                {
                    var name_Of_Class_Or_Package = item.Replace("<y:LabelModel", "");
                    return name_Of_Class_Or_Package;
                }
            }
            return "";
        }
        #endregion
    }
}

