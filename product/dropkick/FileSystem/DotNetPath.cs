﻿// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.FileSystem
{
    using System.IO;
    using System.Security.AccessControl;

    public class DotNetPath :
        Path
    {
        #region Path Members

        public string Combine(string root, string ex)
        {
            return System.IO.Path.Combine(root, ex);
        }

        public string GetFullPath(string path)
        {
            return System.IO.Path.GetFullPath(path);
        }

        public bool IsFile(string path)
        {
            var fi = new FileInfo(GetFullPath(path));
            return (fi.Attributes & FileAttributes.Directory) != FileAttributes.Directory;
        }

        public bool IsDirectory(string path)
        {
            var di = new DirectoryInfo(GetFullPath(path));
            return (di.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        //http://www.west-wind.com/weblog/posts/4072.aspx
        public bool SetTargetSecurity(string target, string group, FileSystemRights permission)
        {
            if (!IsDirectory(target) && !IsFile(target))
                return false;

            var oldSecurity = Directory.GetAccessControl(target);
            var newSecurity = new DirectorySecurity();

            newSecurity.SetSecurityDescriptorBinaryForm(oldSecurity.GetSecurityDescriptorBinaryForm());

            var accessRule = new FileSystemAccessRule(group,
                                                      permission,
                                                      InheritanceFlags.None,
                                                      PropagationFlags.NoPropagateInherit,
                                                      AccessControlType.Allow);
            bool result;
            newSecurity.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

            if (!result)
                return false;

            accessRule = new FileSystemAccessRule(group, 
                                                  permission,
                                                  InheritanceFlags.ContainerInherit | 
                                                  InheritanceFlags.ObjectInherit,
                                                  PropagationFlags.InheritOnly, 
                                                  AccessControlType.Allow);

            result = false;
            newSecurity.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);
            if (!result)
                return false;

            Directory.SetAccessControl(target, newSecurity);
            return true;
        }

        #endregion
    }
}