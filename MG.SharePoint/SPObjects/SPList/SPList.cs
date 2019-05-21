using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPList : SPSecurable
    {
        private List _list;
        private static readonly string[] SkipThese = new string[1] { "SchemaXml" };

        #region CONSTRUCTORS
        public SPList(string listName)
            : this(FindRealListByName(listName))
        {
        }
        internal SPList(List list)
            : base(list)
        {
            base.FormatObject(list, SkipThese);
            this.Name = list.Title;
            _list = list;
        }

        #endregion

        #region METHODS

        public SPListItem AddItem(ListItemCreationInformation info)
        {
            ListItem li = _list.AddItem(info);
            return new SPListItem(li);
        }
        public SPListItem AddItemUsingPath(ListItemCreationInformationUsingPath parameters)
        {
            ListItem li = _list.AddItemUsingPath(parameters);
            return new SPListItem(li);
        }
        public SPListItem CreateDocument(string name, SPFolder targetFolder, DocumentTemplateType templateType)
        {
            ListItem li = _list.CreateDocument(name, (Folder)targetFolder.ShowOriginal(), templateType);
            return new SPListItem(li);
        }
        public ClientResult<string> CreateDocumentAndGetEditLink(string fileName, string folderPath, int documentTemplateType, string templateUrl)
        {
            return _list.CreateDocumentAndGetEditLink(fileName, folderPath, documentTemplateType, templateUrl);
        }
        public SPListItem CreateDocumentFromTemplate(string fileName, SPFolder targetFolder, string templateUrl)
        {
            ListItem li = _list.CreateDocumentFromTemplate(fileName, (Folder)targetFolder.ShowOriginal(), templateUrl);
            return new SPListItem(li);
        }
        public SPListItem CreateDocumentFromTemplateBytes(string fileName, SPFolder targetFolder, byte[] templateBytes, string extension)
        {
            ListItem li = _list.CreateDocumentFromTemplateBytes(fileName, (Folder)targetFolder.ShowOriginal(), templateBytes, extension);
            return new SPListItem(li);
        }
        public SPListItem CreateDocumentFromTemplateStream(string fileName, SPFolder targetFolder, string extension, System.IO.Stream stream)
        {
            ListItem li = _list.CreateDocumentFromTemplateStream(fileName, (Folder)targetFolder.ShowOriginal(), extension, stream);
            return new SPListItem(li);
        }
        public SPListItem CreateDocumentFromTemplateUsingPath(ResourcePath filePath, SPFolder targetFolder, ResourcePath templatePath)
        {
            ListItem li = _list.CreateDocumentFromTemplateUsingPath(filePath, (Folder)targetFolder.ShowOriginal(), templatePath);
            return new SPListItem(li);
        }
        public ClientResult<string> CreateDocumentWithDefaultName(string folderPath, string extension)
        {
            return _list.CreateDocumentWithDefaultName(folderPath, extension);
        }
        public View CreateMappedView(AppViewCreationInfo appViewCreationInfo, VisualizationAppTarget visualizationTarget)
        {
            return _list.CreateMappedView(appViewCreationInfo, visualizationTarget);
        }
        public void DeleteObject() => _list.DeleteObject();
        public ListBloomFilter GetBloomFilter(int startItemId) => _list.GetBloomFilter(startItemId);
        public ListBloomFilter GetBloomFilterWithCustomFields(int startItemId, IList<string> internalFieldNames)
        {
            return _list.GetBloomFilterWithCustomFields(startItemId, internalFieldNames);
        }
        public ChangeCollection GetChanges(ChangeQuery query) => _list.GetChanges(query);
        public CheckedOutFileCollection GetCheckedOutFiles() => _list.GetCheckedOutFiles();
        public SPListItem GetItemById(int id)
        {
            SPListItem spli = null;
            ListItem li = _list.GetItemById(id);
            if (li != null)
                spli = new SPListItem(li);

            return spli;
        }
        public SPListItem GetItemById(string id)
        {
            SPListItem spli = null;
            ListItem li = _list.GetItemById(id);
            if (li != null)
                spli = new SPListItem(li);

            return spli;
        }
        public SPListItem GetItemByUniqueId(Guid id)
        {
            SPListItem spli = null;
            ListItem li = _list.GetItemByUniqueId(id);
            if (li != null)
                spli = new SPListItem(li);

            return spli;
        }
        public VisualizationAppSynchronizationResult GetMappedApp(Guid appId, VisualizationAppTarget target) =>
            _list.GetMappedApp(appId, target);

        public VisualizationAppSynchronizationResult GetMappedApps(VisualizationAppTarget target) =>
            _list.GetMappedApps(target);

        public RelatedFieldCollection GetRelatedFields() => _list.GetRelatedFields();
        public ClientResult<string> GetSpecialFolderUrl(SpecialFolderType type, bool forceCreated, Guid existingGuid) =>
            _list.GetSpecialFolderUrl(type, forceCreated, existingGuid);

        public ClientResult<BasePermissions> GetUserEffectivePermissions(string userName) =>
            _list.GetUserEffectivePermissions(userName);

        public View GetView(Guid viewGuid) => _list.GetView(viewGuid);
        public ClientResult<string> GetWebDavUrl(string sourceUrl) => _list.GetWebDavUrl(sourceUrl);
        public bool IsObjectPropertyInstantiated(string propertyName) => _list.IsObjectPropertyInstantiated(propertyName);
        public View PublishMappedView(Guid appId, VisualizationAppTarget target) => _list.PublishMappedView(appId, target);
        public ClientResult<Guid> Recycle() => _list.Recycle();
        public void RefreshLoad() => _list.RefreshLoad();
        public ClientResult<string> RenderExtendedListFormData(int itemId, string formId, int mode, RenderListFormDataOptions options, int cutOffVersion) =>
            _list.RenderExtendedListFormData(itemId, formId, mode, options, cutOffVersion);

        public ClientResult<System.IO.Stream> RenderListContextMenuData(RenderListContextMenuDataParameters parameters) =>
            _list.RenderListContextMenuData(parameters);

        public ClientResult<string> RenderListData(string viewXml) => _list.RenderListData(viewXml);
        public ClientResult<System.IO.Stream> RenderListDataAsStream(RenderListDataParameters parameters, RenderListDataOverrideParameters or) =>
            _list.RenderListDataAsStream(parameters, or);

        public ClientResult<System.IO.Stream> RenderListFilterData(RenderListFilterDataParameters parameters) =>
            _list.RenderListFilterData(parameters);

        public ClientResult<string> RenderListFormData(int itemId, string formId, int mode) => _list.RenderListFormData(itemId, formId, mode);
        public ClientResult<int> ReserveListItemId() => _list.ReserveListItemId();
        public ClientResult<string> SaveAsNewView(string oldName, string newName, bool isPrivate, string uri) =>
            _list.SaveAsNewView(oldName, newName, isPrivate, uri);
        public void SaveAsTemplate(string fileName, string strName, string description, bool saveData) =>
            _list.SaveAsTemplate(fileName, strName, description, saveData);
        public void SetExemptFromBlockDownloadOfNonViewableFiles(bool value) => _list.SetExemptFromBlockDownloadOfNonViewableFiles(value);
        public override ClientObject ShowOriginal() => _list;
        public FlowSynchronizationResult SyncFlowCallbackUrl(string flowId) => _list.SyncFlowCallbackUrl(flowId);
        public FlowSynchronizationResult SyncFlowInstance(Guid flowId) => _list.SyncFlowInstance(flowId);
        public FlowSynchronizationResult SyncFlowInstances() => _list.SyncFlowInstances();
        public FlowSynchronizationResult SyncFlowTemplates(string category) => _list.SyncFlowTemplates(category);
        public View UnpublishMappedView(Guid appId, VisualizationAppTarget target) => _list.UnpublishMappedView(appId, target);
        public override void Update() => _list.Update();
        public VisualizationAppSynchronizationResult ValidateAppName(string displayName) => _list.ValidateAppName(displayName);

        #endregion

        #region STATIC METHODS

        private static List FindRealListByName(string listName)
        {
            //_list
            if (listName.Contains("/") && !listName.StartsWith("/"))
            {
                listName = "/" + listName;
            }

            var allLists = CTX.SP1.Web.Lists;
            CTX.Lae(allLists, true, ls => ls.Include(
                    l => l.Title, l => l.RootFolder.ServerRelativeUrl
                )
            );
            return allLists.Single(
                l => l.Title.Equals(listName, StringComparison.InvariantCultureIgnoreCase) ||
                l.RootFolder.ServerRelativeUrl.Equals(listName, StringComparison.InvariantCultureIgnoreCase));
        }
        public static SPList FindListByName(string listName) => new SPList(FindRealListByName(listName));

        #endregion

        #region OPERATORS
        public static explicit operator SPList(List realList) => new SPList(realList);

        public static explicit operator SPList(string listName) => new SPList(listName);

        #endregion
    }
}
