using System;
using System.Collections.Generic;

namespace sha1Compare
{
    public class FileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSha1 { get; set; } = string.Empty;
    }
    
    public class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo x, FileInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.FileName == y.FileName && x.FileSha1 == y.FileSha1;
        }

        public int GetHashCode(FileInfo obj)
        {
            unchecked
            {
                var hashCode = (obj.FileName != null ? obj.FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.FileSha1 != null ? obj.FileSha1.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}