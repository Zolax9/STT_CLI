using System;
using System.Collections.Generic;
using System.Linq;

namespace STT_CLI
{
    class Program
    {
        static STT stt;
        static string input;
        static List<string> types = new List<string> { "ty", "recordType", "r", "record", "c", "category", "tc", "recordTypeCategory", "ta", "recordTag", "rt", "recordToRecordTag" };
        static bool cancel; // if 'cancel' is inputted

        static void Main(string[] args)
        {
            stt = new STT(args[0]);
            input = "";

            while (input != "exit")
            {
                input = AskStr("> ");

                switch (input)
                {
                    case string n when (n == "a" || n == "add"):
                        cancel = false;
                        Add(out cancel);
                        break;

                    case string n when (n == "f" || n == "flush"):
                        stt.Flush();
                        break;

                    case string n when (n == "h" || n == "help"):
                        Console.WriteLine("actions:\n  a, add / a\t\t\tadd a type\n  e, exit\t\t\texit CLI\n  f, flush\t\t\tsave all changes to 'stt_out.backup'\n  h, help\t\t\tdisplay this help information\n\ntypes:\n  ty, recordType\t\ttype of record\n  r, record\t\t\tsingle record\n  c, category\t\t\tgroups of record types (WIP)\n  tc, recordTypeCategory\tassigns a record type to a category (WIP)\n  ta, recordTag\t\t\ttags appliable to records (WIP)\n  rt, recordToRecordTag\t\tassigns a record tag to a record");
                        break;
                }
            }
        }

        static void Add(out bool cancel)
        {
            string input = "";
            cancel = false;

            while (!types.Any(n => n == input) && !cancel) {input = AskStr("type? ", out cancel); }
            if (cancel) { return; }

            switch (input)
            {
                case string n when (n == "ty" || n == "recordType"):
                    string name;
                    string icon;
                    int color;
                    string color_int;
                    int hidden;
                    string goal_time;

                    name = AskStr("name? ", out cancel);
                    if (cancel) { return; }
                    icon = AskStr("icon? ");
                    color = AskInt("color? ", out cancel);
                    if (cancel) { return; }
                    color_int = AskStr("color_int? ");
                    hidden = AskInt("hidden? ", out cancel);
                    if (cancel) { return; }
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

                case string n when (n == "r" || n == "record"):
                    string recordType_name = "\t"; // prevent existing record type being used (can't have tabs)
                    int type_id;
                    long time_started = -1;
                    long time_ended = -1;
                    string comment = "";

                    while (!stt.newRecordTypes.Any(n => n.name == recordType_name) && !stt.recordTypes.Any(n => n.name == recordType_name) && !cancel) { recordType_name = AskStr("recordType? ", out cancel); }
                    if (cancel) { return; }
                    switch (stt.newRecordTypes.Any(n => n.name == recordType_name))
                    {
                        case true: // not an original recordType (added via cli and not flushed yet)
                            type_id = stt.newRecordTypes[stt.newRecordTypes.FindIndex(0, stt.newRecordTypes.Count, n => n.name == recordType_name)].id;
                            break;

                        default:
                            type_id = stt.recordTypes[stt.recordTypes.FindIndex(0, stt.recordTypes.Count, n => n.name == recordType_name)].id;
                            break;
                    }
                    time_started = AskLong("start (unix)? ", out cancel);
                    if (cancel) { return; }
                    while (time_ended <= time_started && !cancel) { time_ended = AskLong("end (unix)? ", out cancel); }
                    if (cancel) { return; }
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

                case string n when (n == "rt" || n == "recordToRecordTag"):
                    int record_id = -1;
                    string recordTag_name = "\t";
                    int record_tag_id = -1;

                    while (!stt.newRecords.Any(n => n.id == record_id) && !stt.records.Any(n => n.id == record_id) && !cancel) { record_id = AskInt("record_id? ", out cancel); }
                    if (cancel) { return; }
                    while (!stt.recordTags.Any(n => n.name == recordTag_name) && !cancel) { recordTag_name = AskStr("recordTag? ", out cancel); }
                    if (cancel) { return; }
                    record_tag_id = stt.recordTags[stt.recordTags.FindIndex(0, stt.recordTags.Count, n => n.name == recordTag_name)].id;
                    stt.Add_recordToRecordTag(
                        record_id,
                        record_tag_id
                    );
                    break;

                case "":
                    cancel = true;
                    return;
            }
        }

        static string AskStr(string prompt, out bool cancel)
        {
            cancel = false;
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (input == "") { cancel = true; }
            return input;
        }
        static string AskStr(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static int AskInt(string prompt, out bool cancel)
        {
            string input;
            int output;
            cancel = false;

            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (Int32.TryParse(input, out output)) { return output; }
                if (input == "")
                {
                    cancel = true;
                    return 0;
                }
            }
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
        static long AskLong(string prompt, out bool cancel)
        {
            string input;
            long output;
            cancel = false;

            while (true)
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (long.TryParse(input, out output) && output >= 0) { return output; }
                if (input == "")
                {
                    cancel = true;
                    return 0;
                }
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
