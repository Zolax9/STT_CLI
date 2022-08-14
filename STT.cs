using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STT_CLI
{
    public class STT
    {
        string file;

        public List<recordType> recordTypes;
        public List<record> records;
        public List<category> categories;
        public List<recordTypeCategory> recordTypeCategories;
        public List<recordTag> recordTags;
        public List<recordToRecordTag> recordToRecordTags;

        public List<recordType> newRecordTypes;
        public List<record> newRecords;
        public List<recordToRecordTag> newRecordToRecordTags;

        public int topRecordTypeID; // highest record type number (new records increment from here on STT)
        public int topRecordID; // highest record number

        public STT(string _file)
        {
            recordTypes = new List<recordType>();
            records = new List<record>();
            categories = new List<category>();
            recordTypeCategories = new List<recordTypeCategory>();
            recordTags = new List<recordTag>();
            recordToRecordTags = new List<recordToRecordTag>();

            newRecordTypes = new List<recordType>();
            newRecords = new List<record>();
            newRecordToRecordTags = new List<recordToRecordTag>();

            file = _file;
            string[] contents = File.ReadAllLines(file);
            int i = 1; // first line is header
            string[] line;

            while (contents[i].Substring(0, "recordType\t".Length) == "recordType\t")
            {
                line = contents[i].Split('\t');
                recordTypes.Add(new recordType(
                    int.Parse(line[1]),
                    line[2],
                    line[3],
                    int.Parse(line[4]),
                    line[5],
                    int.Parse(line[6]),
                    line[7]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            topRecordTypeID = recordTypes.Max(n => n.id);
            while (contents[i].Substring(0, "record\t".Length) == "record\t")
            {
                line = contents[i].Split('\t');
                records.Add(new record(
                    int.Parse(line[1]),
                    int.Parse(line[2]),
                    long.Parse(line[3]),
                    long.Parse(line[4]),
                    line[5]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            topRecordID = records.Max(n => n.id);
            while (contents[i].Substring(0, "category\t".Length) == "category\t")
            {
                line = contents[i].Split('\t');
                categories.Add(new category(
                    int.Parse(line[1]),
                    line[2],
                    int.Parse(line[3]),
                    line[4]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            while (contents[i].Substring(0, "typeCategory\t".Length) == "typeCategory\t")
            {
                line = contents[i].Split('\t');
                recordTypeCategories.Add(new recordTypeCategory(
                    int.Parse(line[1]),
                    int.Parse(line[2])
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            while (contents[i].Substring(0, "recordTag\t".Length) == "recordTag\t")
            {
                line = contents[i].Split('\t');
                recordTags.Add(new recordTag(
                    int.Parse(line[1]),
                    int.Parse(line[2]),
                    line[3],
                    int.Parse(line[4]),
                    line[5],
                    line[6]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            while (contents[i].Substring(0, "recordToRecordTag\t".Length) == "recordToRecordTag\t")
            {
                line = contents[i].Split('\t');
                recordToRecordTags.Add(new recordToRecordTag(
                    int.Parse(line[1]),
                    int.Parse(line[2])
                ));
                
                i++;
                if (i == contents.Length) { break; }
            }
        }

        public void Add_recordType(int id, string name, string icon, int color, string color_int, int hidden, string goal_time)
        {
            newRecordTypes.Add(new recordType(
                id,
                name,
                icon,
                color,
                color_int,
                hidden,
                goal_time
            ));
            topRecordTypeID++;
        }
        public void Add_record(int id, int type_id, long time_started, long time_ended, string comment)
        {
            newRecords.Add(new record(
                id,
                type_id,
                time_started,
                time_ended,
                comment
            ));
            topRecordID++;
        }
        public void Add_recordToRecordTag(int record_id, int record_tag_id)
        {
            newRecordToRecordTags.Add(new recordToRecordTag(
                record_id,
                record_tag_id
            ));
        }

        public void Flush()
        {
            List<string> contents = File.ReadAllLines(file).ToList();
            int i = contents.Count;
            
            for (int j = newRecordToRecordTags.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "recordToRecordTag\t{0}\t{1}",
                    newRecordToRecordTags[j].record_id,
                    newRecordToRecordTags[j].record_tag_id
                ));
                recordToRecordTags.Insert(recordToRecordTags.Count - newRecordToRecordTags.Count, newRecordToRecordTags[j]);
                Console.WriteLine(String.Format(
                    "recordToRecordTag\t{0}\t{1}",
                    newRecordToRecordTags[j].record_id,
                    newRecordToRecordTags[j].record_tag_id
                ));
            }
            i -= recordToRecordTags.Count - newRecordToRecordTags.Count;
            i -= recordTags.Count;
            i -= recordTypeCategories.Count;
            i -= categories.Count;
            for (int j = newRecords.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    newRecords[j].id,
                    newRecords[j].type_id,
                    newRecords[j].time_started,
                    newRecords[j].time_ended,
                    newRecords[j].comment
                )); 
                records.Insert(records.Count - newRecords.Count, newRecords[j]);
                Console.WriteLine(String.Format(
                    "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    newRecords[j].id,
                    newRecords[j].type_id,
                    newRecords[j].time_started,
                    newRecords[j].time_ended,
                    newRecords[j].comment
                )); 
            }
            i -= records.Count - newRecords.Count;
            for (int j = newRecordTypes.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    newRecordTypes[j].id,
                    newRecordTypes[j].name,
                    newRecordTypes[j].icon,
                    newRecordTypes[j].color,
                    newRecordTypes[j].color_int,
                    newRecordTypes[j].hidden,
                    newRecordTypes[j].goal_time
                )); 
                recordTypes.Insert(recordTypes.Count - newRecordTypes.Count, newRecordTypes[j]);
                Console.WriteLine(String.Format(
                    "recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    newRecordTypes[j].id,
                    newRecordTypes[j].name,
                    newRecordTypes[j].icon,
                    newRecordTypes[j].color,
                    newRecordTypes[j].color_int,
                    newRecordTypes[j].hidden,
                    newRecordTypes[j].goal_time
                )); 
            }
            i -= recordTypes.Count - newRecordTypes.Count;

            File.WriteAllLines("stt_out.backup", contents);
            file = "stt_out.backup";

            newRecordTypes = new List<recordType>();
            newRecords = new List<record>();
            newRecordToRecordTags = new List<recordToRecordTag>();
        }

        public string BlankToZero(string val) // some values are blank, so need to be parsable to an integer (in this case, zero)
        {
            switch (val)
            {
                case "":
                    return "0";

                default:
                    return val;
            }
        }
    }
}
