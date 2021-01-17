using System;

namespace Unity.PerformanceTesting.Data
{
    [Serializable]
    public class Editor
    {
        public string Version;
        public string Branch;
        public string Changeset;
        public int Date;
    }
}
