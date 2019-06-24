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
        private const string VIEW = "View";
        private const string QUERY = "Query";
        private const string WHERE = "Where";
        private const string EQ = "Eq";
        private const string FIELD_REF = "FieldRefName";
        private const string TEXT = "Text";
        private const string TYPE = "Type";
        private const string VALUE = "Value";

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS
        public Query() : base() { }

        public Query(IDictionary dict)
            : base()
        {
            IEnumerable<string> keys = dict.Keys.Cast<string>();
            JObject Template = this.GetTemplate();
            foreach (string key in keys)
            {
                Template[VIEW][QUERY][WHERE][EQ].Value<JObject>().Add(FIELD_REF, key);
                Template[VIEW][QUERY][WHERE][EQ].Value<JObject>().Add(VALUE, JToken.FromObject(dict[key]));
            }
            XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(Template));
            XmlNodeList eleList = xmlDoc.GetElementsByTagName("Value");
            foreach (XmlElement ele in eleList)
            {
                ele.SetAttribute(TYPE, TEXT);
            }
            base.ViewXml = this.FormatOneLineXml(xmlDoc);
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
                    QuoteChar = char.Parse("'")
                })
                {

                    doc.WriteTo(xtw);
                    return sb.ToString();
                }
            }
        }

        private JObject GetTemplate()
        {
            return new JObject(
                    new JProperty("View",
                        new JObject(
                            new JProperty("Query",
                                new JObject(
                                    new JProperty("Where",
                                        new JObject(
                                            new JProperty("Eq", new JObject()))))))));
        }

        #endregion
    }
}