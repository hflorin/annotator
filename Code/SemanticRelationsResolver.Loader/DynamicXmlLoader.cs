namespace SemanticRelationsResolver.Loaders
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class DynamicXmlLoader : IResourceLoader
    {
        public async Task<dynamic> LoadAsync(string path)
        {
            var document = Task.Run(() => XDocument.Load(new StreamReader(path)));

            dynamic root = new ExpandoObject();

            Parse(root, (await document).Elements().First());

            return root;
        }

        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasAttributes)
            {
                ParseElement(parent, node);
            }
            else if (node.HasElements)
            {
                foreach (var element in node.Elements())
                {
                    if (element.HasAttributes)
                    {
                        ParseElement(parent, element);
                    }
                    else if (element.HasElements)
                    {
                        var firstChildElementName = element.Name.LocalName;

                        if (element.Elements(firstChildElementName).Count() > 1)
                        {
                            ParseList(parent, element);
                        }
                        else
                        {
                            ParseElement(parent, element);
                        }
                    }
                    else
                    {
                        AddProperty(parent, element.Name.ToString(), element.Value.Trim());
                    }
                }
            }
            else if (node.HasAttributes)
            {
                ParseElement(parent, node);
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void ParseElement(dynamic parent, XElement node)
        {
            var item = new ExpandoObject();

            foreach (var attribute in node.Attributes())
            {
                AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
            }

            foreach (var element in node.Elements())
            {
                Parse(item, element);
            }

            AddProperty(parent, node.Name.ToString(), item);
        }

        private static void ParseList(dynamic parent, XElement node)
        {
            var item = new ExpandoObject();
            var list = new List<dynamic>();
            foreach (var element in node.Elements())
            {
                Parse(list, element);
            }

            AddProperty(item, node.Elements().First().Name.LocalName, list);
            AddProperty(parent, node.Name.ToString(), item);
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            var list = parent as List<dynamic>;

            if (list != null)
            {
                list.Add(value);
            }
            else
            {
                var dictionary = parent as IDictionary<string, object>;

                if (dictionary == null)
                {
                    return;
                }

                if (dictionary.ContainsKey(name))
                {
                    var existingList = dictionary[name] as List<dynamic>;

                    if (existingList != null)
                    {
                        existingList.Add(value);
                        return;
                    }

                    var newList = new List<dynamic> {dictionary[name], value};
                    dictionary[name] = newList;
                }
                else
                {
                    dictionary[name] = value;
                }
            }
        }
    }
}