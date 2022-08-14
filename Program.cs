using System;
using System.Collections.Generic;
using System.Linq;

namespace STT_CLI
{
    class Program
    {
        static STT stt;
        static string input;
        static List<string> types = new List<string> { "recordType", "record", "category", "recordTypeCategory", "recordTag", "recordToRecordTag" };

        static void Main(string[] args)
        {
            stt = new STT(args[0]);
            input = "";

            while (input != "exit")
            {
                input = AskStr("> ");

                switch (input)
                {
                    case "add":
                        Add();
                        break;

                    case "flush":
                        stt.Flush();
                        break;
                }
            }
        }

        static void Add()
        {
            string input = "";

            while (!types.Any(n => n == input)) {input = AskStr("type? "); }

            switch (input)
            {
                case "recordType":
                    string name;
                    string icon;
                    int color;
                    string color_int;
                    int hidden;
                    string goal_time;

                    name = AskStr("name? ");
                    icon = AskStr("icon? ");
                    color = AskInt("color? ");
                    color_int = AskStr("color_int? ");
                    hidden = AskInt("hidden? ");
                    goal_time = AskStr("goal_time? ");
                    stt.Add_recordType(
                        stt.topRecordTypeID + 1,
                        name,
                        icon,
                        color,
                        color_int,
                        hidden,
                        goal_time
                    );
                    Console.WriteLine(String.Format("recordType_id: {0}", stt.topRecordTypeID));
                    break;

                case "record":
                    string recordType_name = "\t"; // prevent existing record type being used (can't have tabs)
                    int type_id;
                    long time_started = -1;
                    long time_ended = -1;
                    string comment = "";

                    while (!stt.newRecordTypes.Any(n => n.name == recordType_name) && !stt.recordTypes.Any(n => n.name == recordType_name)) { recordType_name = AskStr("recordType? "); }
                    switch (stt.newRecordTypes.Any(n => n.name == recordType_name))
                    {
                        case true: // not an original recordType (added via cli and not flushed yet)
                            type_id = stt.newRecordTypes[stt.newRecordTypes.FindIndex(0, stt.newRecordTypes.Count, n => n.name == recordType_name)].id;
                            break;

                        default:
                            type_id = stt.recordTypes[stt.recordTypes.FindIndex(0, stt.recordTypes.Count, n => n.name == recordType_name)].id;
                            break;
                    }
                    time_started = AskLong("start (unix)? ");
                    while (time_ended <= time_started) { time_ended = AskLong("end (unix)? "); }
                    comment = AskStr("comment? ");
                    stt.Add_record(
                        stt.topRecordID + 1,
                        type_id,
                        time_started,
                        time_ended,
                        comment
                    );
                    Console.WriteLine(String.Format("record_id: {0}", stt.topRecordID));
                    break;

                case "recordToRecordTag":
                    int record_id = -1;
                    string recordTag_name = "\t";
                    int record_tag_id = -1;

                    while (!stt.newRecords.Any(n => n.id == record_id) && !stt.records.Any(n => n.id == record_id)) { record_id = AskInt("record_id? "); }
                    while (!stt.recordTags.Any(n => n.name == recordTag_name)) { recordTag_name = AskStr("recordTag? "); }
                    record_tag_id = stt.recordTags[stt.recordTags.FindIndex(0, stt.recordTags.Count, n => n.name == recordTag_name)].id;
                    stt.Add_recordToRecordTag(
                        record_id,
                        record_tag_id
                    );
                    break;
            }
        }

        static string AskStr(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static int AskInt(string prompt)
        {
            string input;
            int output;

            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (Int32.TryParse(input, out output)) { return output; }
            }
        }
        static long AskLong(string prompt)
        {
            string input;
            long output;

            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (long.TryParse(input, out output) && output >= 0) { return output; }
            }
        }
    }
}
