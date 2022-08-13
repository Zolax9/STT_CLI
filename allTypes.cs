using System;

namespace STT_CLI
{
    public struct recordType
    {
        public recordType(int _num, string _title, string _icon)
        {
            num = _num;
            title = _title;
            icon = _icon;
        }
        public int num;
        public string title;
        public string icon;
    }
    public struct record
    {
        public record(int _num, int _recordType_num, long _timeFrom, long _timeTo, string _comment) // current Unix time needs 41 unsigned bits (more than int32 can hold)
        {
            num = _num;
            recordType_num = _recordType_num;
            timeFrom = _timeFrom; 
            timeTo = _timeTo;
            comment = _comment;
        }
        public int num;
        public int recordType_num;
        public long timeFrom;
        public long timeTo;
        public string comment;
    }
    public struct category
    {
        public category(int _num, string _title)
        {
            num = _num;
            title = _title;
        }
        public int num;
        public string title;
    } 
    public struct typeCategory
    {
        public typeCategory(int _recordType_num, int _category_num)
        {
            recordType_num = _recordType_num;
            category_num = _category_num;
        }
        public int recordType_num;
        public int category_num;
    }
    public struct recordTag
    {
        public recordTag(int _num, int _recordType_num, string _title)
        {
            num = _num;
            recordType_num = _recordType_num;
            title = _title;
        }
        public int num;
        public int recordType_num;
        public string title;
    }
    public struct recordToRecordTag
    {
        public recordToRecordTag(long _record_num, int _recordTag_num)
        {
            record_num = _record_num;
            recordTag_num = _recordTag_num;
        }
        public long record_num;
        public int recordTag_num;
    }
}
