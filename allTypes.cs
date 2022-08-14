using System;

namespace STT_CLI
{
    public struct recordType
    {
        public recordType(int _id, string _name, string _icon, int _color, string _color_int, int _hidden, string _goal_time)
        {
            id = _id;
            name = _name;
            icon = _icon;
            color = _color;
            color_int = _color_int;
            hidden = _hidden;
            goal_time = _goal_time;
        }
        public int id;
        public string name;
        public string icon;
        public int color;
        public string color_int;
        public int hidden;
        public string goal_time; // can be blank ("") if no goal time is set, so string variable used
    }
    public struct record
    {
        public record(int _id, int _type_id, long _time_started, long _time_ended, string _comment) // current Unix time needs 41 unsigned bits (more than int32 can hold)
        {
            id = _id;
            type_id = _type_id;
            time_started = _time_started; 
            time_ended = _time_ended;
            comment = _comment;
        }
        public int id;
        public int type_id;
        public long time_started;
        public long time_ended;
        public string comment;
    }
    public struct category
    {
        public category(int _id, string _name, int _color, string _color_int)
        {
            id = _id;
            name = _name;
            color = _color;
            color_int = _color_int;
        }
        public int id;
        public string name;
        public int color;
        public string color_int;
    } 
    public struct recordTypeCategory
    {
        public recordTypeCategory(int _record_type_id, int _category_id)
        {
            record_type_id = _record_type_id;
            category_id = _category_id;
        }
        public int record_type_id;
        public int category_id;
    }
    public struct recordTag
    {
        public recordTag(int _id, int _type_id, string _name, int _color, string _color_int, string _archived)
        {
            id = _id;
            type_id = _type_id;
            name = _name;
            color = _color;
            color_int = _color_int;
            archived = _archived;
        }
        public int id;
        public int type_id;
        public string name;
        public int color;
        public string color_int;
        public string archived; // can be blank ("") if no goal time is set, so string variable used
    }
    public struct recordToRecordTag
    {
        public recordToRecordTag(long _record_id, int _record_tag_id)
        {
            record_id = _record_id;
            record_tag_id = _record_tag_id;
        }
        public long record_id;
        public int record_tag_id;
    }
}
