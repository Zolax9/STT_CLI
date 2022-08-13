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
        public List<typeCategory> typeCategories;
        public List<recordTag> recordTags;
        public List<recordToRecordTag> recordToRecordTags;

        public List<record> newRecords;
        public List<recordToRecordTag> newRecordToRecordTags;

        public int topRecordNum;

        public STT(string _file)
        {
            recordTypes = new List<recordType>();
            records = new List<record>();
            categories = new List<category>();
            typeCategories = new List<typeCategory>();
            recordTags = new List<recordTag>();
            recordToRecordTags = new List<recordToRecordTag>();

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
                    line[3]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
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
            topRecordNum = records.Max(n => n.num);
            while (contents[i].Substring(0, "category\t".Length) == "category\t")
            {
                line = contents[i].Split('\t');
                categories.Add(new category(
                    int.Parse(line[1]),
                    line[2]
                ));

                i++;
                if (i == contents.Length) { break; }
            }
            while (contents[i].Substring(0, "typeCategory\t".Length) == "typeCategory\t")
            {
                line = contents[i].Split('\t');
                typeCategories.Add(new typeCategory(
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
                    line[3]
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

        public void Add_record(int num, int recordType_num, long timeFrom, long timeTo, string comment)
        {
            newRecords.Add(new record(
                num,
                recordType_num,
                timeFrom,
                timeTo,
                comment
            ));
            topRecordNum++;
        }
        public void Add_recordToRecordTag(int record_num, int recordTag_num)
        {
            newRecordToRecordTags.Add(new recordToRecordTag(
                record_num,
                recordTag_num
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
                    newRecordToRecordTags[j].record_num,
                    newRecordToRecordTags[j].recordTag_num
                ));
                recordToRecordTags.Insert(recordToRecordTags.Count - newRecordToRecordTags.Count, newRecordToRecordTags[j]);
                Console.WriteLine(String.Format(
                    "recordToRecordTag\t{0}\t{1}",
                    newRecordToRecordTags[j].record_num,
                    newRecordToRecordTags[j].recordTag_num
                ));
            }
            i -= recordToRecordTags.Count - newRecordToRecordTags.Count;
            i -= recordTags.Count;
            i -= typeCategories.Count;
            i -= categories.Count;
            for (int j = newRecords.Count - 1; j >= 0; j--)
            {
                contents.Insert(i, String.Format(
                    "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    newRecords[j].num,
                    newRecords[j].recordType_num,
                    newRecords[j].timeFrom,
                    newRecords[j].timeTo,
                    newRecords[j].comment
                )); 
                records.Insert(records.Count - newRecords.Count, newRecords[j]);
                Console.WriteLine(String.Format(
                    "record\t{0}\t{1}\t{2}\t{3}\t{4}",
                    newRecords[j].num,
                    newRecords[j].recordType_num,
                    newRecords[j].timeFrom,
                    newRecords[j].timeTo,
                    newRecords[j].comment
                )); 
            }
            i -= records.Count - newRecords.Count;
            i -= recordTypes.Count;

            File.WriteAllLines("stt_out.backup", contents);
            file = "stt_out.backup";

            newRecords = new List<record>();
            newRecordToRecordTags = new List<recordToRecordTag>();
        }
    }
}
