using System;

namespace STT_CLI
{
    public struct recordType
    {
        int num;
        string title;
        string icon;
    }
    public struct record
    {
        int num;
        int recordType_num;
        int timeFrom;
        int timeTo;
        string comment;
    }
    public struct category
    {
        int num;
        string title;
    }
    public struct typeCategory
    {
        int recordType_num;
        int category_num;
    }
    public struct recordTag
    {
        int num;
        int recordType_num;
        int title;
    }
    public struct recordToRecordTag
    {
        int record_num;
        int recordTag_num;
    }
}
