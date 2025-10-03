using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvHelper;
using Horiba.Sdk.Data;
using Serilog;

namespace HelperFunctions
{
    public static class CsvParser
    {
        public static void SaveCcdAcquisitionDataToCsv(CcdData ccdData, string csvFilename)
        {

            // Extract xData and yData
            var xData = ccdData.Acquisition[0].Region[0].XData;
            var yData = ccdData.Acquisition[0].Region[0].YData[0];

            // Write to CSV file
            using (var file = new StreamWriter(csvFilename))
            {
                using (var writer = new CsvWriter(file, System.Globalization.CultureInfo.InvariantCulture))
                {
                    writer.WriteField("xData");
                    writer.WriteField("yData");
                    writer.NextRecord();

                    for (int i = 0; i < xData.Count; i++)
                    {
                        writer.WriteField(xData[i]);
                        writer.WriteField(yData[i]);
                        writer.NextRecord();
                    }
                }
            }
        }
        

        public static void SaveSpectrAcq3DataToCsv(List<(int Wavelength, DataItem DataItem)> dataItems,
            string csvFilename)
        {

            var data = new List<DataItem>
            {};

            List<DataItem> onlyDataItems = dataItems.Select(item => item.DataItem).ToList();

            List<string> headers = onlyDataItems
                .SelectMany(item =>
                    item.GetType()
                        .GetProperties()
                        .Where(p =>
                            p.PropertyType == typeof(SaqSignal) &&
                            p.GetValue(item) != null
                        )
                        .SelectMany(p =>
                        {
                            var outerName = p.Name;
                            var saqSignal = p.GetValue(item);
                            return saqSignal.GetType()
                                .GetProperties()
                                .Select(np => $"{outerName}.{np.Name}");
                        })
                )
                .Distinct()
                .ToList();

            headers.Insert(0, "wavelength");

            

            // Write to a single CSV file
            using (var file = new StreamWriter(csvFilename))
            {
                using (var writer = new CsvWriter(file, System.Globalization.CultureInfo.InvariantCulture))
                {
                    // Write headers
                    foreach (var header in headers)
                    {
                        writer.WriteField(header);
                    }

                    writer.NextRecord();

                    // Write data rows
                    foreach (var (wavelength, dataItem) in dataItems)
                    {
                        var row = new List<object>
                        {wavelength};

                        if (headers.Any(header => header.Contains("Current")))
                        {
                            row.Add(dataItem.CurrentSignal.Value);
                            row.Add(dataItem.CurrentSignal.Unit);
                        }

                        if (headers.Any(header => header.Contains("Pmt")))
                        {
                            row.Add(dataItem.PmtSignal.Value);
                            row.Add(dataItem.PmtSignal.Unit);
                        }

                        if (headers.Any(header => header.Contains("Ppd")))
                        {
                            row.Add(dataItem.PpdSignal.Value);
                            row.Add(dataItem.PpdSignal.Unit);
                        }

                        if (headers.Any(header => header.Contains("Voltage")))
                        {
                            row.Add(dataItem.VoltageSignal.Value);
                            row.Add(dataItem.VoltageSignal.Unit);
                        }


                        foreach (var field in row)
                        {
                            writer.WriteField(field);
                        }

                        writer.NextRecord();
                    }
                }
            }
            Log.Information($"Data saved to {csvFilename}");
        }
    }
}   
