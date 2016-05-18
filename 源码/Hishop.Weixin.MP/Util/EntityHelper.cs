namespace Hishop.Weixin.MP.Util
{
    using Hishop.Weixin.MP.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public static class EntityHelper
    {
        public static XDocument ConvertEntityToXml<T>(T entity) where T : class, new()
        {
            if (entity == null)
            {
            }
            entity = Activator.CreateInstance<T>();
            XDocument document = new XDocument();
            document.Add(new XElement("xml"));
            XElement root = document.Root;
            List<string> list = new string[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Content", "ArticleCount", "Articles", "FuncFlag", "Title ", "Description ", "PicUrl", "Url" }.ToList<string>();
            Func<string, int> orderByPropName = new Func<string, int>(list.IndexOf);
            List<PropertyInfo> list2 = (from p in entity.GetType().GetProperties()
                                        orderby orderByPropName(p.Name)
                                        select p).ToList<PropertyInfo>();
            foreach (PropertyInfo info in list2)
            {
                DateTime time;
                string name = info.Name;
                if (name == "Articles")
                {
                    XElement content = new XElement("Articles");
                    List<Article> list3 = info.GetValue(entity, null) as List<Article>;
                    foreach (Article article in list3)
                    {
                        IEnumerable<XElement> enumerable = ConvertEntityToXml<Article>(article).Root.Elements();
                        content.Add(new XElement("item", enumerable));
                    }
                    root.Add(content);
                }
                else
                {
                    string str2 = info.PropertyType.Name;
                    if (str2 == null)
                    {
                        root.Add(new XElement(name, info.GetValue(entity, null)));
                    }
                    if (!(str2 == "String"))
                    {
                        if (str2 == "DateTime")
                        {
                            time = (DateTime)info.GetValue(entity, null);
                            root.Add(new XElement(name, time.Ticks));
                            continue;
                        }
                        if (str2 == "Boolean")
                        {
                            if (name == "FuncFlag")
                            {
                                root.Add(new XElement(name, ((bool)info.GetValue(entity, null)) ? "1" : "0"));
                                continue;
                            }
                        }
                        if (str2 == "ResponseMsgType")
                        {
                            root.Add(new XElement(name, info.GetValue(entity, null).ToString().ToLower()));
                            continue;
                        }
                        if (str2 == "Article")
                        {
                            root.Add(new XElement(name, info.GetValue(entity, null).ToString().ToLower()));
                            continue;
                        }
                        root.Add(new XElement(name, info.GetValue(entity, null)));
                    }
                    root.Add(new XElement(name, new XCData((info.GetValue(entity, null) as string) ?? "")));
                }
                continue;
            }
            return document;
        }

        public static void FillEntityWithXml<T>(T entity, XDocument doc) where T : AbstractRequest, new()
        {
            entity = Activator.CreateInstance<T>();
            XElement root = doc.Root;
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                string name = info.Name;
                if (root.Element(name) != null)
                {
                    string str2 = info.PropertyType.Name;
                    if (str2 == null)
                    {
                        info.SetValue(entity, root.Element(name).Value, null);
                    }
                    if (!(str2 == "DateTime"))
                    {
                        if (str2 == "Boolean")
                        {
                            if (name == "FuncFlag")
                            {
                                info.SetValue(entity, root.Element(name).Value == "1", null);
                                continue;
                            }
                        }
                        if (str2 == "Int64")
                        {
                            info.SetValue(entity, long.Parse(root.Element(name).Value), null);
                            continue;
                        }
                        if (str2 == "RequestEventType")
                        {
                            info.SetValue(entity, EventTypeHelper.GetEventType(root.Element(name).Value), null);
                            continue;
                        }
                        if (str2 == "RequestMsgType")
                        {
                            info.SetValue(entity, MsgTypeHelper.GetMsgType(root.Element(name).Value), null);
                            continue;
                        }
                        info.SetValue(entity, root.Element(name).Value, null);
                    }
                    info.SetValue(entity, new DateTime(long.Parse(root.Element(name).Value)), null);
                }
                continue;
            }
        }
    }
}
