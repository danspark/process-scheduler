using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ProcessScheduler.Core.Csv
{
    public class ProcessParser
    {
        public static IEnumerable<Process> ParseProcesses(string file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                
            };

            config.RegisterClassMap<ProcessCsvMap>();
            config.Delimiter = ";";
            config.HasHeaderRecord = false;

            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<Process>().ToList();
        }
    }

    public class ProcessCsvMap : ClassMap<Process>
    {
        public ProcessCsvMap()
        {
            Map(p => p.User).Index(0);
            Map(p => p.Name).Index(1);
            Map(p => p.Id).Index(2);
            Map(p => p.SubmissionTime).ConvertUsing(r => TimeSpan.FromMilliseconds(r.GetField<int>(3)));
            Map(p => p.TotalExecutionTime).ConvertUsing(r => TimeSpan.FromMilliseconds(r.GetField<int>(4)));
        }
    }
}
