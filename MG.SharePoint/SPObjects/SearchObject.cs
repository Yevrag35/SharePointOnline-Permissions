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
        public Type TypedObject { get; }

        #endregion

        #region CONSTRUCTORS
        public SearchObject(File spFile)
        {
            this.Name = spFile.Name;
            this.Id = spFile.UniqueId;
            this.TypedObject = FILE_TYPE;
            _cliObj = spFile;
        }
        public SearchObject(Folder spFolder)
        {
            this.Name = spFolder.Name;
            this.Id = spFolder.UniqueId;
            this.TypedObject = FOLDER_TYPE;
            _cliObj = spFolder;
        }

        #endregion

        #region PUBLIC METHODS
        public bool NameMatchesId(params Guid[] ids)
        {
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
        public bool NameMatchesPattern(params WildcardPattern[] wcps)
        {
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
        public bool NameMatchesString(params string[] strs)
        {
            if (strs == null)
                throw new ArgumentNullException("Strings");

            var pats = new WildcardPattern[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                pats[i] = new WildcardPattern(strs[i], WildcardOptions.IgnoreCase);
            }

            return this.NameMatchesPattern(pats);
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
}