using System;
using System.Collections.Generic;
using System.Linq;

namespace STT_CLI
{
    class Program
    {
        static STT stt;
        static string input;
        // static List<string> types = new List<string> { "ty", "recordType", "r", "record", "c", "category", "tc", "recordTypeCategory", "ta", "recordTag", "rt", "recordToRecordTag" };
        static List<string> types = new List<string> {
            "ty", "recordType",
             "r", "record",
             "rt", "recordToRecordTag"
        }; // types supported so far
        static bool cancel; // if 'cancel' is inputted

        static void Main(string[] args)
        {
            stt = new STT(args[0]);
            input = "";

            while (input != "exit" && input != "e")
            {
                input = AskStr("> ");

                switch (input)
                {
                    case string n when (n == "a" || n == "add"):
                        cancel = false;
                        Add(out cancel);
                        break;

                    case string n when (n == "d" || n == "delete"):
                        cancel = false;
                        Delete(out cancel);
                        break;

                    case string n when (n == "f" || n == "flush"):
                        stt.Flush();
                        break;

                    case string n when (n == "h" || n == "help"):
                        Console.WriteLine("actions:\n  a, add / a\t\t\tadd a type\n  d, delete\t\t\tdelete a type\n  x, exit\t\t\texit CLI\n  f, flush\t\t\tsave all changes to 'stt_out.backup'\n  h, help\t\t\tdisplay this help information\n\ntypes:\n  ty, recordType\t\ttype of record\n  r, record\t\t\tsingle record\n  c, category\t\t\tgroups of record types (WIP)\n  tc, recordTypeCategory\tassigns a record type to a category (WIP)\n  ta, recordTag\t\t\ttags appliable to records (WIP)\n  rt, recordToRecordTag\t\tassigns a record tag to a record");
                        break;
                }
            }
        }

        static void Add(out bool cancel)
        {
            string input = "";
            bool exit = false;
            bool exit2 = false; // for use in double while loops

            cancel = false;
            while (
                !types.Any(n => n == input) &&
                !cancel
            ) {input = AskStr("type? ", out cancel); }
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

                    while (!exit)
                    {
                        name = AskStr("name? ", out cancel);
                        if (cancel) { return; }
                        icon = AskStr("icon? ");
                        color = AskInt("color? ", out cancel);
                        if (cancel) { return; }
                        color_int = AskStr("color_int? ");
                        hidden = AskInt("hidden? ", out cancel);
                        if (cancel) { return; }
                        goal_time = AskStr("goal_time? ");
                    
                        switch (stt.Add_recordType(
                            stt.top_recordType_id + 1,
                            name,
                            icon,
                            color,
                            color_int,
                            hidden,
                            goal_time
                        )) {
                            case true:
                                exit = true;
                                break;

                            case false:
                                Console.WriteLine("you need to flush first. (recordType set for deletion has the same name)");
                                break;
                        }
                    }
                    Console.WriteLine(String.Format("recordType_id: {0}", stt.top_recordType_id));
                    break;

                case string n when (n == "r" || n == "record"):
                    string recordType_name = "\n"; // prevent existing record type being used (can't have newlines)
                    int type_id;
                    long time_started = -1;
                    long time_ended = -1;
                    string comment = "";

                    // too many conditions to stay and escape to loop while on all conditions
                    while (!exit)
                    {
                        recordType_name = AskStr("recordType? ", out cancel);
                        if (cancel) { exit = true; }
                        if (!stt.delete_recordTypes.Any(n => stt.recordTypes[n].name == recordType_name))
                        {
                            if (
                                stt.add_recordTypes.Any(n => n.name == recordType_name) ||
                                stt.recordTypes.Any(n => n.name == recordType_name)
                            ) { exit = true; }
                        }
                    }
                    if (cancel) { return; }
                    switch (stt.add_recordTypes.Any(n => n.name == recordType_name))
                    {
                        case true: // not an original recordType (added via cli and not flushed yet)
                            type_id = stt.add_recordTypes[stt.add_recordTypes.FindIndex(0, stt.add_recordTypes.Count, n => n.name == recordType_name)].id;
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
                        stt.top_record_id + 1,
                        type_id,
                        time_started,
                        time_ended,
                        comment
                    );
                    Console.WriteLine(String.Format("record_id: {0}", stt.top_record_id));
                    break;

                case string n when (n == "rt" || n == "recordToRecordTag"):
                    int record_id = -1;
                    int record_tag_id = -1;

                    while (!exit2)
                    {
                        exit = false;
                        while (!exit)
                        {
                            record_id = AskInt("record_id? ", out cancel);
                            if (!cancel) { exit = true; }
                            if (!stt.delete_records.Any(n => stt.records[n].id == record_id))
                            {
                                if (
                                    stt.add_records.Any(n => n.id == record_id) ||
                                    stt.records.Any(n => n.id == record_id)
                                ) { exit = true; }
                            }
                        }
                        if (cancel) { return; }

                        exit = false;
                        while (!exit)
                        {

                        }

                        while (
                            !stt.recordTags.Any(n => n.id == record_tag_id) &&
                            !cancel
                        ) { record_tag_id = AskInt("record_tag_id? ", out cancel); }
                        if (cancel) { return; }

                        switch (stt.Add_recordToRecordTag(
                            record_id,
                            record_tag_id
                        ))
                        {
                            case true:
                                exit2 = true;
                                break;

                            case false:
                                Console.WriteLine("you need to flush first. (recordToRecordTag set for deletion has same properties)");
                                break;
                        }
                    }
                    break;

                case "":
                    cancel = true;
                    return;
            }
        }

        static void Delete(out bool cancel)
        {
            string input = "";
            cancel = false;
            while (
                !types.Any(n => n == input) &&
                !cancel
            ) {input = AskStr("type? ", out cancel); }
            if (cancel) { return; }

            int id;
            switch (input)
            {
                case string n when (n == "ty" || n == "recordType"):
                    id = -1;
                    string name;

                    while (
                        !stt.add_recordTypes.Any(n => n.id == id) &&
                        !stt.recordTypes.Any(n => n.id == id) &&
                        !cancel
                    ) { id = AskInt("id? ", out cancel); }
                    if (cancel) { return; }
                    switch (stt.add_recordTypes.Any(n => n.id == id))
                    {
                        case true: //  deleting a new recordType (added via cli and not flushed yet)
                            name = stt.add_recordTypes[stt.add_recordTypes.FindIndex(0, stt.add_recordTypes.Count, n => n.id == id)].name;
                            Console.WriteLine(String.Format("deleting \"{0}\"", name));
                            stt.Delete_add_recordType(stt.add_recordTypes.FindIndex(0, stt.add_recordTypes.Count, n => n.id == id));
                            break;

                        default:
                            name = stt.recordTypes[stt.recordTypes.FindIndex(0, stt.recordTypes.Count, n => n.id == id)].name;
                            Console.WriteLine(String.Format("deleting \"{0}\"", name));
                            stt.Delete_recordType(stt.recordTypes.FindIndex(0, stt.recordTypes.Count, n => n.id == id));
                            break;
                    }
                    break;

                case string n when (n == "r" || n == "record"):
                    id = -1;

                    while (
                        !stt.add_records.Any(n => n.id == id) &&
                        !stt.records.Any(n => n.id == id) &&
                        !cancel
                    ) { id = AskInt("id? ", out cancel); }
                    if (cancel) { return; }
                    switch (stt.add_records.Any(n => n.id == id))
                    {
                        case true:
                            stt.Delete_add_record(stt.add_records.FindIndex(0, stt.add_records.Count, n => n.id == id));
                            break;

                        default:
                            stt.Delete_record(stt.records.FindIndex(0, stt.records.Count, n => n.id == id));
                            break;
                    }
                    break;

                case string n when (n == "rt" || n == "recordToRecordTag"):
                    int record_id = -1;
                    int record_tag_id = -1;

                    while (
                        !stt.add_records.Any(n => n.id == record_id) &&
                        !stt.records.Any(n => n.id == record_id) &&
                        !cancel
                    ) { record_id = AskInt("record_id? ", out cancel); }
                    if (cancel) { return; }
                    while (
                        !stt.recordTags.Any(n => n.id == record_tag_id) &&
                        !cancel
                    ) { record_tag_id = AskInt("record_tag_id? ", out cancel); }
                    if (cancel) { return; }
                    switch (stt.add_recordToRecordTags.Any(n => n.record_id == record_id && n.record_tag_id == record_tag_id))
                    {
                        case true:
                            stt.Delete_add_recordToRecordTag(stt.add_recordToRecordTags.FindIndex(0, stt.add_recordToRecordTags.Count, n => n.record_id == record_id && n.record_tag_id == record_tag_id));
                            break;

                        default:
                            stt.Delete_recordToRecordTag(stt.recordToRecordTags.FindIndex(0, stt.recordToRecordTags.Count, n => n.record_id == record_id && n.record_tag_id == record_tag_id));
                            break;
                    }
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
