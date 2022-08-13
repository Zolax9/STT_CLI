using System;
using System.Collections.Generic;
using System.Linq;

namespace STT_CLI
{
    class Program
    {
        static STT stt;
        static string input;
        static List<string> types = new List<string> { "recordType", "record", "category", "typeCategory", "recordTag", "recordToRecordTag" };

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
                case "record":
                    string recordType = "\t"; // prevent existing record type being used (can't have tabs)
                    int recordType_num;
                    long timeFrom = -1;
                    long timeTo = -1;
                    string comment = "";

                    while (!stt.recordTypes.Any(n => n.title == recordType)) { recordType = AskStr("recordType? "); }
                    recordType_num = stt.recordTypes[stt.recordTypes.FindIndex(0, stt.recordTypes.Count - 1, n => n.title == recordType)].num;
                    timeFrom = AskLong("from (unix)? ");
                    while (timeTo <= timeFrom) { timeTo = AskLong("to (unix)? "); }
                    comment = AskStr("comment? ");
                    stt.Add_record(
                        stt.topRecordNum + 1,
                        recordType_num,
                        timeFrom,
                        timeTo,
                        comment
                    );
                    Console.WriteLine(String.Format("record number: {0}", stt.topRecordNum));
                    break;

                case "recordToRecordTag":
                    int record_num = -1;
                    string recordTag = "\t";
                    int recordTag_num = -1;

                    while (!stt.newRecords.Any(n => n.num == record_num) && !stt.records.Any(n => n.num == record_num)) { record_num = AskInt("record_num? "); }
                    while (!stt.recordTags.Any(n => n.title == recordTag)) { recordTag = AskStr("recordTag? "); }
                    recordTag_num = stt.recordTags[stt.recordTags.FindIndex(0, stt.recordTags.Count - 1, n => n.title == recordTag)].num;
                    stt.Add_recordToRecordTag(
                        record_num,
                        recordTag_num
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
