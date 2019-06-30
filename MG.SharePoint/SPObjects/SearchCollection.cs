using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SearchCollection : ICollection<SearchObject>
    {
        #region FIELDS/CONSTANTS
        private readonly List<SearchObject> _list;

        #endregion

        #region PROPERTIES
        public bool ContainsFiles => _list.Count > 0 && _list.Exists(x => x.TypedObject.Equals(SearchObject.FILE_TYPE));
        public bool ContainsFolders => _list.Count > 0 && _list.Exists(x => x.TypedObject.Equals(SearchObject.FOLDER_TYPE));
        public int Count => _list.Count;
        public bool IsReadOnly => false;

        #endregion

        #region CONSTRUCTORS
        public SearchCollection() => _list = new List<SearchObject>();
        public SearchCollection(int capacity) => _list = new List<SearchObject>(capacity);
        public SearchCollection(IEnumerable<SearchObject> objs) => _list = new List<SearchObject>(objs);

        #endregion

        #region PUBLIC METHODS
        public void Add(File file) => _list.Add(new SearchObject(file));
        public void Add(Folder folder) => _list.Add(new SearchObject(folder));
        public void Add(SearchObject item) => _list.Add(item);
        public void AddFromFileCollection(FileCollection fileCol)
        {
            fileCol.Context.Load(fileCol, fc => fc.Include(f => f.Name, f => f.UniqueId));
            fileCol.Context.ExecuteQuery();

            for (int i = 0; i < fileCol.Count; i++)
            {
                _list.Add(new SearchObject(fileCol[i]));
            }
        }
        public void AddFromFolderCollection(FolderCollection folderCol)
        {
            folderCol.Context.Load(folderCol, fc => fc.Include(f => f.Name, f => f.UniqueId));
            folderCol.Context.ExecuteQuery();

            for (int i = 0; i < folderCol.Count; i++)
            {
                _list.Add(new SearchObject(folderCol[i]));
            }
        }
        public void Clear() => _list.Clear();
        public bool Contains(SearchObject item) => _list.Contains(item);
        public void CopyTo(SearchObject[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public bool Exists(Predicate<SearchObject> match) => _list.Exists(match);
        public ClientObject Find(Predicate<SearchObject> match) => _list.Find(match).ReturnLoaded();
        public ClientObject[] FindAll(Predicate<SearchObject> match)
        {
            List<SearchObject> matchedObjs = _list.FindAll(match);
            var retArr = new ClientObject[matchedObjs.Count];
            for (int i = 0; i < matchedObjs.Count; i++)
            {
                retArr[i] = matchedObjs[i].ReturnLoaded();
            }
            return retArr;
        }
        public IEnumerator<SearchObject> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        public bool Remove(SearchObject item) => _list.Remove(item);
        public override string ToString() => string.Join(", ", _list.Select(x => x.Name));
        public bool TrueForAll(Predicate<SearchObject> match) => _list.TrueForAll(match);

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}