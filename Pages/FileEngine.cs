using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public sealed partial class Engine : ScrollView
    {
        public void SetData(string ext)
        {
            if (ext == ".json")
            {
                this.Content = new TreeView();
            }
            else
            {
                this.Content = new TextBlock();
            }
        }

        public void SetText(string content)
        {
            if (Content is TextBlock block)
            {
                block.Text = content;
            }
            else if (Content is TreeView treeView)
            {
                LoadTree(treeView, content);
            }
        }

        public void SetFontFamily(FontFamily font)
        {
            if (Content is TextBlock block)
            {
                block.FontFamily = font;
            }
        }

        public void SetFontSize(int size)
        {
            if (Content is TextBlock block)
            {
                block.FontSize = size;
            }
        }

        public void LoadTree(TreeView obj, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            obj.RootNodes.Clear();

            JContainer json;
            try
            {
                if (content.StartsWith('['))
                {
                    json = JArray.Parse(content);
                    obj.RootNodes.Add(JsonToTree((JArray)json, "Root"));
                }
                else
                {
                    json = JObject.Parse(content);
                    obj.RootNodes.Add(JsonToTree((JObject)json, "Root"));
                }
            }
            catch (JsonReaderException)
            {
                // invalid json
            }
        }

        public TreeViewNode JsonToTree(JArray obj, string nodeName)
        {
            if (obj == null)
                return null;

            string itemCountString = obj.Count.ToString() + " item" + (obj.Count > 1 ? "s" : "");
            TreeViewNode parent = new() { Content = $"{nodeName}:  {itemCountString}" };

            foreach ((JToken token, int index) in obj.Select((value, i) => (value, i)))
            {
                if (token.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)token, $"{nodeName}[{index}]"));
                }
                else if (token.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)token, $"{nodeName}[{index}]"));
                }
                else
                {
                    parent.Children.Add(new TreeViewNode()
                    {
                        Content = $"{nodeName}[{index}]:  {token}"
                    });
                }
            }

            return parent;
        }

        public TreeViewNode JsonToTree(JObject obj, string nodeName)
        {
            if (obj == null)
                return null;

            TreeViewNode parent = new() { Content = $"{nodeName}:  {obj.Count} items" };

            foreach (KeyValuePair<string, JToken> pair in obj)
            {
                if (pair.Value.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)pair.Value, pair.Key));
                }
                else if (pair.Value.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)pair.Value, pair.Key));
                }
                else
                {
                    parent.Children.Add(new TreeViewNode()
                    {
                        Content = $"{pair.Key}:  {pair.Value}"
                    });
                }
            }

            return parent;
        }
    }
}