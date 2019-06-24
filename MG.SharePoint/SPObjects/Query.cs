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
        private const string ID = "ID";
        private const string NAME = "Name";
        private const string TEXT = "Text";
        private const string TYPE = "Type";
        private const string VALUE = "Value";

        private const string EX_MSG = "\"{0}\" is not a valid field for the given FieldCollection.";

        private readonly FieldCollection _fieldCol;

        #endregion

        #region PROPERTIES


        #endregion

        #region CONSTRUCTORS
        public Query() : base() { }

        public Query(FieldCollection fields)
            : base() => _fieldCol = fields;

        public Query(IDictionary dict, FieldCollection fields)
            : this(fields)
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
                string value = Convert.ToString(dict[key]);

                if (key.Equals(FILE, StringComparison.CurrentCultureIgnoreCase))
                    key = FILE_LEAF_REF;

                else if (key.Equals(ID, StringComparison.CurrentCultureIgnoreCase))
                    key = ID;

                Field realField = this.ResolveFieldInternalName(key);
                if (realField == null)
                    this.ThrowArgumentException(key);

                XmlNode eq = parent.AppendChild(xmlDoc.CreateElement(EQ));

                var ele = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(FIELD_REF));
                ele.SetAttribute(NAME, realField.InternalName);

                var val = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(VALUE));
                val.SetAttribute(TYPE, realField.TypeAsString);
                val.AppendChild(xmlDoc.CreateTextNode(value));
            }

            this.ViewXml = this.FormatOneLineXml(xmlDoc);
        }

        public Query(string fieldName, IEnumerable<string> col, FieldCollection fields)
            : this(fields)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");

            else if (fieldName.Equals(FILE, StringComparison.CurrentCultureIgnoreCase))
                fieldName = FILE_LEAF_REF;

            else if (fieldName.Equals(ID, StringComparison.CurrentCultureIgnoreCase))
                fieldName = ID;

            Field realField = this.ResolveFieldInternalName(fieldName);
            if (realField == null)
                this.ThrowArgumentException(fieldName);

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

                ele.SetAttribute(NAME, realField.InternalName);

                var val = (XmlElement)eq.AppendChild(xmlDoc.CreateElement(VALUE));
                val.SetAttribute(TYPE, realField.TypeAsString);

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

        private Field ResolveFieldInternalName(string possible)
        {
            if (_fieldCol == null)
                throw new InvalidOperationException("No fields have been specified for this query.");

            Field field = _fieldCol.GetByInternalNameOrTitle(possible);
            field.Context.Load(field, f => f.InternalName, f => f.TypeAsString);
            try
            {
                field.Context.ExecuteQuery();
                return field;
            }
            catch (ServerException)
            {
                return null;
            }
        }

        private void ThrowArgumentException(string fieldName) => throw new ArgumentException(string.Format(EX_MSG, fieldName));

        #endregion
    }
}