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

        public int topRecordNum;

        public STT(string file)
        {
            recordTypes = new List<recordType>();
            records = new List<record>();
            categories = new List<category>();
            typeCategories = new List<typeCategory>();
            recordTags = new List<recordTag>();
            recordToRecordTags = new List<recordToRecordTag>();

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
            Console.WriteLine(topRecordNum);
        }
    }
}
