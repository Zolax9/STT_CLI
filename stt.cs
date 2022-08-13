using System;
using System.Collections.Generic;

namespace STT_CLI
{
    public class STT
    {
        string file;

        public List<recordType> recordTypes;
        public List<record> records;
        public List<category> categories;
        public List<typeCategory> typeCategories;
        public List<recordTag> recordTags;
        public List<recordToRecordTag> recordToRecordTags;

        public STT()
        {
        }
    }
}
