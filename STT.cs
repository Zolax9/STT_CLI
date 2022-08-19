using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STT_CLI
{
    public class STT
    {
        string file; // name of input backup file
        // imported types
        public List<recordType> recordTypes;
        public List<record> records;
        public List<category> categories;
        public List<recordTypeCategory> recordTypeCategories;
        public List<recordTag> recordTags;
        public List<recordToRecordTag> recordToRecordTags;

        // these are added via CLI but haven't been flushed yet
        public List<recordType> add_recordTypes;
        public List<record> add_records;
        public List<recordToRecordTag> add_recordToRecordTags;

        // these are to be deleted by CLI but haven't been flushed yet
        public List<int> delete_recordTypes; // both recordTypes and records have ids, so only need one property per type
        public List<int> delete_records;
        public List<int> delete_recordToRecordTags;

        // highest ids for each type with ids (new types increment from here on STT)
        public int top_recordType_id;
        public int top_record_id;
        public int top_category_id;
        public int top_recordTag_id;

        public STT(string _file)
        {
            recordTypes = new List<recordType>();
            records = new List<record>();
            categories = new List<category>();
            recordTypeCategories = new List<recordTypeCategory>();
            recordTags = new List<recordTag>();
            recordToRecordTags = new List<recordToRecordTag>();

            add_recordTypes = new List<recordType>();
            add_records = new List<record>();
            add_recordToRecordTags = new List<recordToRecordTag>();

            delete_recordTypes = new List<int>();
            delete_records = new List<int>();
            delete_recordToRecordTags = new List<int>();

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
            top_recordType_id = recordTypes.Max(n => n.id);
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
            top_record_id = records.Max(n => n.id);
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
            top_category_id = categories.Max(n => n.id);
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
            top_recordTag_id = recordTags.Max(n => n.id);
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

        // true = successful, false = fail (overlapping property (usually name) with non-flushed deletion)
        public bool Add_recordType(int id, string name, string icon, int color, string color_int, int hidden, string goal_time)
        {
            switch (delete_recordTypes.Any(n => recordTypes[n].name == name))
            {
                case true:
                    return false;

                case false:
                    add_recordTypes.Add(new recordType(
                        id,
                        name,
                        icon,
                        color,
                        color_int,
                        hidden,
                        goal_time
                    ));
                    top_recordType_id++;
                    return true;
            }
        }
        public void Add_record(int id, int type_id, long time_started, long time_ended, string comment)
        {
            add_records.Add(new record(
                id,
                type_id,
                time_started,
                time_ended,
                comment
            ));
            top_record_id++;
        }
        public bool Add_recordToRecordTag(int record_id, int record_tag_id)
        {
            switch (
                delete_recordToRecordTags.Any(n => recordToRecordTags[n].record_id == record_id) &&
                delete_recordToRecordTags.Any(n => recordToRecordTags[n].record_tag_id == record_tag_id)
            ) {
                case true:
                    return false;

                case false:
                    add_recordToRecordTags.Add(new recordToRecordTag(
                        record_id,
                        record_tag_id
                    ));
                    return true;
            }
        }
        public void Delete_recordType(int elem)
        {
            // if the highest id of a type is deleted and needs to be changed
            if (recordTypes[elem].id == top_recordType_id)
            {
                // emulates recordTypes without recordTypes set to be deleted (can't be included in getting top ID
                List<recordType> temp = new List<recordType>();
                temp.AddRange(recordTypes);

                // removing at a list of elements requires a descending order
                delete_recordTypes.Sort();
                delete_recordTypes.Reverse();

                for (int i = 0; i < delete_recordTypes.Count; i++) { temp.RemoveAt(delete_recordTypes[i]); }
                top_recordType_id = temp.Max(n => n.id);
            }
            delete_recordTypes.Add(elem);
        }
        public void Delete_add_recordType(int elem) { add_recordTypes.RemoveAt(elem); }
        public void Delete_record(int elem)
        {
            if (records[elem].id == top_record_id)
            {
                List<record> temp = new List<record>();
                temp.AddRange(records);

                delete_records.Sort();
                delete_records.Reverse();

                for (int i = 0; i < delete_records.Count; i++) { temp.RemoveAt(delete_records[i]); }
                top_record_id = temp.Max(n => n.id);
            }
            delete_recordTypes.Add(elem);
        }
        public void Delete_add_record(int elem) { add_records.RemoveAt(elem); }
        public void Delete_recordToRecordTag(int elem) { delete_recordTypes.Add(elem); }
        public void Delete_add_recordToRecordTag(int elem) { add_recordTypes.RemoveAt(elem); }

        public void Flush()
        {
            List<string> contents = File.ReadAllLines(file).ToList();
            int i = contents.Count;

            Console.WriteLine("added:");
            for (int j = add_recordToRecordTags.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "recordToRecordTag\t{0}\t{1}",
                    add_recordToRecordTags[j].record_id,
                    add_recordToRecordTags[j].record_tag_id
                ));
                recordToRecordTags.Insert(recordToRecordTags.Count - add_recordToRecordTags.Count, add_recordToRecordTags[j]);
                Console.WriteLine(String.Format(
                    "  recordToRecordTag\t{0}\t{1}",
                    add_recordToRecordTags[j].record_id,
                    add_recordToRecordTags[j].record_tag_id
                ));
            }
            i -= recordToRecordTags.Count - add_recordToRecordTags.Count;
            i -= recordTags.Count;
            i -= recordTypeCategories.Count;
            i -= categories.Count;
            for (int j = add_records.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    add_records[j].id,
                    add_records[j].type_id,
                    add_records[j].time_started,
                    add_records[j].time_ended,
                    add_records[j].comment
                )); 
                records.Insert(records.Count - add_records.Count, add_records[j]);
                Console.WriteLine(String.Format(
                    "  record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    add_records[j].id,
                    add_records[j].type_id,
                    add_records[j].time_started,
                    add_records[j].time_ended,
                    add_records[j].comment
                )); 
            }
            i -= records.Count - add_records.Count;
            for (int j = add_recordTypes.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    add_recordTypes[j].id,
                    add_recordTypes[j].name,
                    add_recordTypes[j].icon,
                    add_recordTypes[j].color,
                    add_recordTypes[j].color_int,
                    add_recordTypes[j].hidden,
                    add_recordTypes[j].goal_time
                )); 
                recordTypes.Insert(recordTypes.Count - add_recordTypes.Count, add_recordTypes[j]);
                Console.WriteLine(String.Format(
                    "  recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    add_recordTypes[j].id,
                    add_recordTypes[j].name,
                    add_recordTypes[j].icon,
                    add_recordTypes[j].color,
                    add_recordTypes[j].color_int,
                    add_recordTypes[j].hidden,
                    add_recordTypes[j].goal_time
                )); 
            }
            i -= recordTypes.Count - add_recordTypes.Count;

            Console.WriteLine("deleted:");
            delete_recordToRecordTags.Sort();
            delete_recordToRecordTags.Reverse();
            for (int j = delete_recordToRecordTags.Count - 1; j >= 0; j--)
            {
                contents.RemoveAt(contents.FindIndex(
                    0,
                    contents.Count,
                    n => n == String.Format(
                        "recordToRecordTag\t{0}\t{1}",
                        recordToRecordTags[delete_recordToRecordTags[j]].record_id,
                        recordToRecordTags[delete_recordToRecordTags[j]].record_tag_id
                    )
                ));
                Console.WriteLine(String.Format(
                    "  recordToRecordTag\t{0}\t{1}",
                    recordToRecordTags[delete_recordToRecordTags[j]].record_id,
                    recordToRecordTags[delete_recordToRecordTags[j]].record_tag_id
                ));
                recordToRecordTags.RemoveAt(delete_recordToRecordTags[j]);
            }
            i -= recordToRecordTags.Count + delete_recordToRecordTags.Count;
            i -= recordTags.Count;
            i -= recordTypeCategories.Count;
            i -= categories.Count;
            // don't need to sort delete_records (already done in delete_record())
            for (int j = delete_records.Count - 1; j >= 0; j--)
            {
                contents.RemoveAt(contents.FindIndex(
                    0,
                    contents.Count,
                    n => n == String.Format(
                        "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                        records[delete_records[j]].id,
                        records[delete_records[j]].type_id,
                        records[delete_records[j]].time_started,
                        records[delete_records[j]].time_ended,
                        records[delete_records[j]].comment
                    )
                ));
                Console.WriteLine(String.Format(
                    "  record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    records[delete_records[j]].id,
                    records[delete_records[j]].type_id,
                    records[delete_records[j]].time_started,
                    records[delete_records[j]].time_ended,
                    records[delete_records[j]].comment
                ));
                records.RemoveAt(delete_records[j]);
            }
            i -= records.Count + delete_records.Count;
            for (int j = delete_recordTypes.Count - 1; j >= 0; j--)
            {
                contents.RemoveAt(contents.FindIndex(
                    0,
                    contents.Count,
                    n => n == String.Format(
                        "recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                        recordTypes[delete_recordTypes[j]].id,
                        recordTypes[delete_recordTypes[j]].name,
                        recordTypes[delete_recordTypes[j]].icon,
                        recordTypes[delete_recordTypes[j]].color,
                        recordTypes[delete_recordTypes[j]].color_int,
                        recordTypes[delete_recordTypes[j]].hidden,
                        recordTypes[delete_recordTypes[j]].goal_time
                    )
                ));
                Console.WriteLine(String.Format(
                    "  recordType\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                    recordTypes[delete_recordTypes[j]].id,
                    recordTypes[delete_recordTypes[j]].name,
                    recordTypes[delete_recordTypes[j]].icon,
                    recordTypes[delete_recordTypes[j]].color,
                    recordTypes[delete_recordTypes[j]].color_int,
                    recordTypes[delete_recordTypes[j]].hidden,
                    recordTypes[delete_recordTypes[j]].goal_time
                )); 
                recordTypes.RemoveAt(delete_recordTypes[j]);
            }
            i -= recordTypes.Count + delete_recordTypes.Count;

            File.WriteAllLines("stt_out.backup", contents);
            file = "stt_out.backup";

            add_recordTypes = new List<recordType>();
            add_records = new List<record>();
            add_recordToRecordTags = new List<recordToRecordTag>();

            delete_recordTypes = new List<int>();
            delete_records = new List<int>();
            delete_recordToRecordTags = new List<int>();
        }
    }
}
