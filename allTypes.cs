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
        int num;
        string title;
        string icon;
    }
    public struct record
    {
        public record(int _num, int _recordType_num, int _timeFrom, int _timeTo, string _comment)
        {
            num = _num;
            recordType_num = _recordType_num;
            timeFrom = _timeFrom;
            timeTo = _timeTo;
            comment = _comment;
        }
        int num;
        int recordType_num;
        int timeFrom;
        int timeTo;
        string comment;
    }
    public struct category
    {
        public category(int _num, string _title)
        {
            num = _num;
            title = _title;
        }
        int num;
        string title;
    }
    public struct typeCategory
    {
        public typeCategory(int _recordType_num, int _category_num)
        {
            recordType_num = _recordType_num;
            category_num = _category_num;
        }
        int recordType_num;
        int category_num;
    }
    public struct recordTag
    {
        public recordTag(int _num, int _recordType_num, int _title)
        {
            num = _num;
            recordType_num = _recordType_num;
            title = _title;
        }
        int num;
        int recordType_num;
        int title;
    }
    public struct recordToRecordTag
    {
        public recordToRecordTag(int _record_num, int _recordTag_num)
        {
            record_num = _record_num;
            recordTag_num = _recordTag_num;
        }
        int record_num;
        int recordTag_num;
    }
}
