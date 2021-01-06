using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYPPlatform.Web.Storage.DocumentDb
{
    public class DocumentDbSettings
    {
        public string EndpointUrl { get; set; }
        public string PrimaryKey { get; set; }
        public int MaxConnectionLimit { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
