using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Lucene.Net.Documents;
using Lucinq.Building;
using Lucinq.SitecoreIntegration.Constants;
using Lucinq.SitecoreIntegration.Extensions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Xml;
using DateField = Sitecore.Data.Fields.DateField;
using Field = Sitecore.Data.Fields.Field;

namespace Lucinq.SitecoreIntegration.Indexing
{
    /// <summary>
    /// This is a helper class to perform basic indexing.
    /// </summary>
    public class IndexingHelper
    {
        #region [ Fields ]

        private static readonly Dictionary<string, string> fieldTypes = new Dictionary<string, string>();

        private static readonly Dictionary<ID, FieldConfiguration> fieldConfigurations = new Dictionary<ID, FieldConfiguration>();

        #endregion

        #region [ Properties ]

        public static Dictionary<string, string> FieldTypes { get { return fieldTypes; } }

        public static Dictionary<ID, FieldConfiguration> FieldConfigurations { get { return fieldConfigurations; } }

        #endregion

        #region [ Methods ]

        public virtual void AddFieldTypes(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");
            var fieldName = XmlUtil.GetAttribute("name", configNode);
            var fieldType = XmlUtil.GetAttribute("type", configNode);
            FieldTypes.Add(fieldName.ToLowerInvariant(), fieldType.ToLower());
        }

        public virtual void AddFieldConfigurations(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");
            var idString = XmlUtil.GetAttribute("id", configNode);
            ID fieldId;
            if (!ID.TryParse(idString, out fieldId))
            {
                return;
            }
            FieldConfiguration fieldConfiguration = new FieldConfiguration();
            bool store;
            fieldConfiguration.Store = bool.TryParse(XmlUtil.GetAttribute("store", configNode), out store) && store;
            bool analyze;
            fieldConfiguration.Analyze = bool.TryParse(XmlUtil.GetAttribute("analyze", configNode), out analyze) && analyze;

            FieldConfigurations.Add(fieldId, fieldConfiguration);
        }

        /// <summary>
        /// Adds the lucinq specific fields
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        protected void AddLucinqFields(Document document, Item item)
        {
            StringBuilder templatePathBuilder = new StringBuilder();
            AddTemplatePath(item.Template, templatePathBuilder);

            StringBuilder pathBuilder = new StringBuilder();
            AddItemPath(item, pathBuilder);

            document.Setup(
                    x => x.AddNonAnalysedField(SitecoreFields.Id, item.ID.ToLuceneId(), true),
                    x => x.AddNonAnalysedField(SitecoreFields.Language, item.Language.Name, true),
                    x => x.AddNonAnalysedField(SitecoreFields.TemplateId, item.TemplateID.ToLuceneId(), true),
                    x => x.AddAnalysedField(SitecoreFields.Name, item.Name, true),
                    x => x.AddAnalysedField(SitecoreFields.TemplatePath, templatePathBuilder.ToString()),
                    x => x.AddAnalysedField(SitecoreFields.Path, pathBuilder.ToString()),
                    x => x.AddAnalysedField(SitecoreFields.Parent, item.ParentID.ToLuceneId())
                );
        }

        /// <summary>
        /// Override this method to add your additional fields
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        protected virtual void AddCustomFields(Document document, Item item)
        {
            // Placeholder method to add additional fields specific to the implementation
        }

        protected virtual void AddItemPath(Item item, StringBuilder stringBuilder)
        {
            if (item.Parent == null)
            {
                return;
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(" ");
            }
            stringBuilder.Append(item.ParentID.ToLuceneId());
            AddItemPath(item.Parent, stringBuilder);
        }

        protected virtual void AddTemplatePath(TemplateItem templateItem, StringBuilder stringBuilder)
        {
            if (templateItem.ID == Sitecore.TemplateIDs.StandardTemplate)
            {
                return;
            }

            stringBuilder.Append(templateItem.ID.ToLuceneId());
            foreach (TemplateItem template in templateItem.BaseTemplates)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" ");
                }
                AddTemplatePath(template, stringBuilder);
            }
        }

        protected virtual void AddField(Document document, Field field)
        {
            string fieldType = "value";
            string fieldTypeKey = field.TypeKey.ToLower();
            if (FieldTypes.ContainsKey(fieldTypeKey))
            {
                fieldType = FieldTypes[fieldTypeKey].ToLower();
            }

            var handled = AddSpecificFieldByTypeKey(document, field, fieldTypeKey);

            if (handled)
            {
                return;
            }
            AddDefaultFieldByType(document, field, fieldType);
        }

        protected virtual void AddDefaultFieldByType(Document document, Field field, string fieldType)
        {
            FieldConfiguration fieldConfiguration;
            // default handling
            switch (fieldType)
            {
                case "multilist":
                    fieldConfiguration = GetFieldConfiguration(field);
                    AddMultilistField(document, field, fieldConfiguration);
                    break;
                case "link":
                    fieldConfiguration = GetFieldConfiguration(field, false);
                    AddLinkField(document, field, fieldConfiguration);
                    break;
                case "datetime":
                    fieldConfiguration = GetFieldConfiguration(field, false);
                    AddDateTimeField(document, field, fieldConfiguration);
                    break;
                default:
                    fieldConfiguration = GetFieldConfiguration(field);
                    AddValueField(document, field, fieldConfiguration);
                    break;
            }
        }

        protected virtual bool AddSpecificFieldByTypeKey(Document document, Field field, string fieldTypeKey)
        {
            FieldConfiguration fieldConfiguration;
            bool handled = false;
            // specific field type handling
            switch (fieldTypeKey)
            {
                case "rich text":
                    fieldConfiguration = GetFieldConfiguration(field);
                    AddValueField(document, field, fieldConfiguration, true);
                    break;
                case "accountsmultilist":
                    fieldConfiguration = GetFieldConfiguration(field);
                    AddMultilistField(document, field, fieldConfiguration);
                    handled = true;
                    break;
                case "date":
                    fieldConfiguration = GetFieldConfiguration(field, false);
                    AddDateTimeField(document, field, fieldConfiguration);
                    handled = true;
                    break;
            }
            return handled;
        }

        protected void AddDateTimeField(Document document, DateField field, FieldConfiguration fieldConfiguration)
        {
            if (field == null || fieldConfiguration == null)
            {
                return;
            }

            string fieldName = SitecoreQueryBuilderExtensions.GetEncodedFieldName(field.InnerField.Name);
            if (fieldConfiguration.Analyze)
            {
                document.AddAnalysedField(fieldName, DateTools.DateToString(field.DateTime, DateTools.Resolution.MINUTE), fieldConfiguration.Store);
                return;
            }

            document.AddNonAnalysedField(fieldName, DateTools.DateToString(field.DateTime, DateTools.Resolution.MINUTE), fieldConfiguration.Store);
        }

        protected virtual void AddLinkField(Document document, LinkField field, FieldConfiguration fieldConfiguration)
        {
            if (field == null || fieldConfiguration == null)
            {
                return;
            }

            string fieldName = SitecoreQueryBuilderExtensions.GetEncodedFieldName(field.InnerField.Name);
            if (fieldConfiguration.Analyze)
            {
                document.AddAnalysedField(fieldName, field.TargetID.ToLuceneId(), fieldConfiguration.Store);
                return;
            }
            document.AddNonAnalysedField(fieldName, field.TargetID.ToLuceneId(), fieldConfiguration.Store);
        }

        protected virtual void AddValueField(Document document, Field field, FieldConfiguration fieldConfiguration, bool stripHtml = false)
        {
            if (field == null || fieldConfiguration == null)
            {
                return;
            }

            ID valueId;
            string fieldValue = field.Value;

            if (stripHtml && !String.IsNullOrEmpty(fieldValue))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(fieldValue);
                fieldValue = htmlDoc.DocumentNode.InnerText;
            }

            if (ID.TryParse(field.Value, out valueId))
            {
                fieldConfiguration.Analyze = false;
                fieldValue = valueId.ToLuceneId();
            }

            string fieldName = SitecoreQueryBuilderExtensions.GetEncodedFieldName(field.Name);
            if (fieldConfiguration.Analyze)
            {
                document.AddAnalysedField(fieldName, fieldValue, fieldConfiguration.Store);
                return;
            }
            document.AddNonAnalysedField(fieldName, fieldValue, fieldConfiguration.Store);
        }

        protected virtual FieldConfiguration GetFieldConfiguration(Field field, bool analyze = true, bool store = false)
        {
            // todo: Allow this to read from the item
            FieldConfiguration fieldConfiguration = new FieldConfiguration { FieldId = field.ID, Analyze = analyze, Store = store };
            if (FieldConfigurations.ContainsKey(field.ID))
            {
                fieldConfiguration = FieldConfigurations[field.ID];
            }
            return fieldConfiguration;
        }

        protected virtual void AddMultilistField(Document document, MultilistField field, FieldConfiguration fieldConfiguration)
        {
            if (field == null || fieldConfiguration == null)
            {
                return;
            }

            StringBuilder fieldBuilder = new StringBuilder();
            bool first = true;
            foreach (ID targetId in field.TargetIDs)
            {
                if (!first)
                {
                    fieldBuilder.Append(" ");
                }
                first = false;
                fieldBuilder.Append(targetId.ToLuceneId());
            }

            string fieldName = SitecoreQueryBuilderExtensions.GetEncodedFieldName(field.InnerField.Name);
            if (fieldConfiguration.Analyze)
            {
                document.AddAnalysedField(fieldName, fieldBuilder.ToString(), fieldConfiguration.Store);
                return;
            }

            document.AddNonAnalysedField(fieldName, fieldBuilder.ToString(), fieldConfiguration.Store);
        }

        #endregion
    }
}
