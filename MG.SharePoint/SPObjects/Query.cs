using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MG.SharePoint
{
    public class Query : CamlQuery
    {
        #region FIELDS/CONSTANTS
        private const string AND = "And";
        private const string OR = "Or";
        private const string VIEW = "View";
        private const string QUERY = "Query";
        private const string WHERE = "Where";
        private const string EQ = "Eq";
        private const string FILE = "File";
        private const string FILE_LEAF_REF = "FileLeafRef";
        private const string FIELD_REF = "FieldRef";
        private const string NAME = "Name";
        private const string TEXT = "Text";
        private const string TYPE = "Type";
        private const string VALUE = "Value";

        private const string EX_MSG = "The string collection needs to contains more than one string.";

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS
        public Query() : base() { }

        public Query(IDictionary dict)
            : base()
        {
            string[] keys = dict.Keys.Cast<string>().ToArray();
            var xmlDoc = new XmlDocument();
            XmlNode view = xmlDoc.AppendChild(xmlDoc.CreateElement(VIEW));
            XmlNode query = view.AppendChild(xmlDoc.CreateElement(QUERY));
            XmlNode where = query.AppendChild(xmlDoc.CreateElement(WHERE));

            XmlNode parent = keys.Length > 1
                ? where.AppendChild(xmlDoc.CreateElement(AND))
                : where;

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                XmlNode eq = parent.AppendChild(xmlDoc.CreateElement(EQ));

                var ele = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(FIELD_REF));
                ele.SetAttribute(NAME, key);
                var val = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(VALUE));
                if (key.Equals(FILE_LEAF_REF, StringComparison.CurrentCultureIgnoreCase))
                {
                    val.SetAttribute(TYPE, FILE);
                }
                else
                {
                    val.SetAttribute(TYPE, TEXT);
                }
                val.AppendChild(xmlDoc.CreateTextNode(Convert.ToString(dict[key])));
            }

            this.ViewXml = this.FormatOneLineXml(xmlDoc);
        }

        public Query(string fieldName, ICollection<string> col)
            : base()
        {
            string[] items = col.ToArray();

            var xmlDoc = new XmlDocument();
            XmlNode view = xmlDoc.AppendChild(xmlDoc.CreateElement(VIEW));
            XmlNode query = view.AppendChild(xmlDoc.CreateElement(QUERY));
            XmlNode where = query.AppendChild(xmlDoc.CreateElement(WHERE));

            XmlNode parent = items.Length > 1
                ? where.AppendChild(xmlDoc.CreateElement(OR))
                : where;

            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                XmlNode eq = parent.AppendChild(xmlDoc.CreateElement(EQ));

                var ele = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(FIELD_REF));
                ele.SetAttribute(NAME, fieldName);

                var val = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(VALUE));

                if (fieldName.Equals(FILE_LEAF_REF, StringComparison.CurrentCultureIgnoreCase))
                {
                    val.SetAttribute(TYPE, FILE);
                }
                else
                {
                    val.SetAttribute(TYPE, TEXT);
                }
                val.AppendChild(xmlDoc.CreateTextNode(item));
            }

            this.ViewXml = this.FormatOneLineXml(xmlDoc);
        }

        #endregion

        #region PUBLIC METHODS


        #endregion

        #region BACKEND/PRIVATE METHODS
        private string FormatOneLineXml(XmlDocument doc)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var xtw = new XmlTextWriter(sw)
                {
                    Formatting = System.Xml.Formatting.None,
                    QuoteChar = char.Parse("\"")
                })
                {

                    doc.WriteTo(xtw);
                    return sb.ToString();
                }
            }
        }

        //private JObject GetTemplate()
        //{
        //    return new JObject(
        //            new JProperty("View",
        //                new JObject(
        //                    new JProperty("Query",
        //                        new JObject(
        //                            new JProperty("Where",
        //                                new JObject(
        //                                    new JProperty("Eq", new JObject()))))))));
        //}

        #endregion
    }
}