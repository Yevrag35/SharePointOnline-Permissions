using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint
{
    public class SearchObject
    {
        #region FIELDS/CONSTANTS
        internal static readonly Type FILE_TYPE = typeof(File);
        internal static readonly Type FOLDER_TYPE = typeof(Folder);

        private readonly ClientObject _cliObj;
        private bool _loaded = false;

        #endregion

        #region PROPERTIES
        public string Name { get; }
        public Guid Id { get; }
        public bool IsLoaded => _loaded;
        public SearchObjectType Type { get; }

        #endregion

        #region CONSTRUCTORS
        public SearchObject(File spFile, bool isLoaded = false)
        {
            if (!isLoaded && !spFile.IsPropertyReady(x => x.Name, x => x.UniqueId))
                spFile.LoadProperty(x => x.Name, x => x.UniqueId);

            _loaded = isLoaded;
            this.Name = spFile.Name;
            this.Id = spFile.UniqueId;
            this.Type = SearchObjectType.File;
            _cliObj = spFile;
        }
        public SearchObject(Folder spFolder, bool isLoaded = false)
        {
            if (!isLoaded && !spFolder.IsPropertyReady(x => x.Name, x => x.UniqueId))
                spFolder.LoadProperty(x => x.Name, x => x.UniqueId);

            _loaded = isLoaded;
            this.Name = spFolder.Name;
            this.Id = spFolder.UniqueId;
            this.Type = SearchObjectType.Folder;
            _cliObj = spFolder;
        }

        #endregion

        #region PUBLIC METHODS
        public bool NameMatchesId(SearchObjectType type, params Guid[] ids)
        {
            if (this.Type != type)
                return false;

            if (ids == null)
                throw new ArgumentNullException("UniqueIds");

            bool result = false;
            for (int i = 0; i < ids.Length; i++)
            {
                if (this.Id.Equals(ids[i]))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool NameMatchesPattern(SearchObjectType type, params WildcardPattern[] wcps)
        {
            if (this.Type != type)
                return false;

            if (wcps == null)
                throw new ArgumentNullException("WildcardPatterns");

            bool result = false;
            for (int i = 0; i < wcps.Length; i++)
            {
                if (wcps[i].IsMatch(this.Name))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        public bool NameMatchesString(SearchObjectType type, params string[] strs)
        {
            if (this.Type != type)
                return false;

            if (strs == null)
                throw new ArgumentNullException("Strings");

            var pats = new WildcardPattern[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                pats[i] = new WildcardPattern(strs[i], WildcardOptions.IgnoreCase);
            }

            return this.NameMatchesPattern(type, pats);
        }

        public ClientObject ReturnLoaded()
        {
            if (!_loaded)
            {
                if (_cliObj is Folder fol)
                {
                    ((Folder)_cliObj).LoadFolderProps();
                }
                else
                {
                    ((File)_cliObj).LoadFileProps();
                }
                _loaded = true;
            }
            return _cliObj;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }

    public class SearchComparer : IComparer<SearchObject>
    {
        public int Compare(SearchObject x, SearchObject y) => x.Name.CompareTo(y.Name);
    }
    public class SearchEquality : IEqualityComparer<SearchObject>
    {
        public bool Equals(SearchObject x, SearchObject y) => x.Id.Equals(y.Id);
        public int GetHashCode(SearchObject obj) => 0;
    }

    public enum SearchObjectType
    {
        File,
        Folder
    }
}