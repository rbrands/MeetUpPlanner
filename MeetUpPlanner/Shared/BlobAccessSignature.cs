using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class BlobAccessSignature
    {
        public Uri Sas { get; set; }
        public string BlobName { get; set; }
        public string FileName { get; set; }

        public BlobAccessSignature()
        {

        }
        public BlobAccessSignature(string blobName, string fileName, Uri sas)
        {
            BlobName = blobName;
            FileName = fileName;
            Sas = sas;
        }
    }
}
