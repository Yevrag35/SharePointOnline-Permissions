using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SearchCollection : BaseSPCollection, ICollection<SearchObject>
    {
        #region FIELDS/CONSTANTS
        private List<SearchObject> _list;

        #endregion

        #region PROPERTIES
        public bool ContainsFiles => _list.Count > 0 && _list.Exists(x => x.Type == SearchObjectType.File);
        public bool ContainsFolders => _list.Count > 0 && _list.Exists(x => x.Type == SearchObjectType.Folder);
        public override int Count => _list.Count;
        public bool IsReadOnly => false;

        #endregion

        #region CONSTRUCTORS
        public SearchCollection()
            : this(CTX.SP1) { }
        public SearchCollection(ClientContext ctx)
            : base(ctx) => _list = new List<SearchObject>();
        public SearchCollection(int capacity)
            : this(CTX.SP1, capacity) { }
        public SearchCollection(ClientContext ctx, int capacity)
            : base(ctx) => _list = new List<SearchObject>(capacity);
        public SearchCollection(IEnumerable<SearchObject> objs)
            : this(CTX.SP1, objs) { }
        public SearchCollection(ClientContext ctx, IEnumerable<SearchObject> objs)
            : base(ctx) => _list = new List<SearchObject>(objs);

        #endregion

        #region INDEXING
        public SearchObject this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        #endregion

        #region PUBLIC METHODS
        public void Add(File file)
        {
            bool loaded = file.IsLoaded();
            _list.Add(new SearchObject(file, loaded));
        }
        public void Add(Folder folder)
        {
            bool loaded = folder.IsLoaded();
            _list.Add(new SearchObject(folder, loaded));
        }
        public void Add(SearchObject item) => _list.Add(item);
        public void AddFileFromFileCollection(FileCollection fileCol)
        {
            fileCol.Context.Load(fileCol, fc => fc.Include(f => f.Name, f => f.UniqueId));
            fileCol.Context.ExecuteQuery();

            for (int i = 0; i < fileCol.Count; i++)
            {
                _list.Add(new SearchObject(fileCol[i]));
            }
        }
        public void AddFileFromFolderCollection(FolderCollection folderCol)
        {
            folderCol.Initialize();
            foreach (Folder topFol in folderCol.Where(fol => !fol.Name.StartsWith("_")))
            {
                this.LoadSubFilesAndFolders(topFol);
            }
            this.RemoveDuplicates();
        }
        public void AddFolderFromFolderCollection(FolderCollection folderCol)
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
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
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
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        IEnumerator<SearchObject> IEnumerable<SearchObject>.GetEnumerator() => _list.GetEnumerator();
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();

        public bool Remove(SearchObject item) => _list.Remove(item);
        public void RemoveDuplicates() => _list = _list.Distinct(new SearchEquality()).ToList();
        public void Sort() => _list.Sort(new SearchComparer());
        public void Sort(IComparer<SearchObject> comparer) => _list.Sort(comparer);
        public override object SyncRoot => ((ICollection)_list).SyncRoot;
        public override string ToString() => string.Join(", ", _list.Select(x => x.Name));
        public bool TrueForAll(Predicate<SearchObject> match) => _list.TrueForAll(match);

        #endregion

        #region BACKEND/PRIVATE METHODS
        private void LoadSubFilesAndFolders(Folder fol)
        {
            fol.Context.Load(fol, x => x.Files.Include(f => f.Name, f => f.UniqueId),
                x => x.Folders.Include(fo => fo.Name, fo => fo.UniqueId));

            try
            {
                fol.Context.ExecuteQuery();
            }
            catch (ServerException)
            {
                return;
            }

            if (fol.Files.Count > 0)
            {
                this.AddFileFromFileCollection(fol.Files);
            }

            if (fol.Folders.Count > 0)
            {
                for (int i = 0; i < fol.Folders.Count; i++)
                {
                    Folder subFol = fol.Folders[i];
                    this.LoadSubFilesAndFolders(subFol);
                    this.Add(subFol);
                }
            }
            this.Add(fol);
        }

        #endregion
    }
}