using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvHelper;
using Horiba.Sdk.Data;

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

        public static void SaveSpectrAcq3DataToCsv(DataItem dataItem, string csvFilename)
        {
            var headers = new List<string>
            {
                "elapsedTime", "currentSignal_value", "currentSignal_unit", "pmtSignal_value", "pmtSignal_unit",
                "ppdSignal_value", "ppdSignal_unit", "voltageSignal_value", "voltageSignal_unit",
                "eventMarker", "overscaleCurrentChannel", "overscaleVoltageChannel", "pointNumber"
            };

            // Write to CSV file
            using (var file = new StreamWriter(csvFilename))
            {
                using (var writer = new CsvWriter(file, System.Globalization.CultureInfo.InvariantCulture))
                {
                    foreach (var header in headers)
                    {
                        writer.WriteField(header);
                    }
                    writer.NextRecord();

                    var row = new List<object>
                    {
                        dataItem.ElapsedTime,
                        dataItem.CurrentSignal.Value, dataItem.CurrentSignal.Unit,
                        dataItem.PmtSignal.Value, dataItem.PmtSignal.Unit,
                        dataItem.PpdSignal.Value, dataItem.PpdSignal.Unit,
                        dataItem.VoltageSignal.Value, dataItem.VoltageSignal.Unit,
                        dataItem.ElapsedTime,
                        dataItem.OverscaleCurrentChannel,
                        dataItem.OverscaleVoltageChannel,
                        dataItem.PointNumber
                    };

                    foreach (var field in row)
                    {
                        writer.WriteField(field);
                    }
                    writer.NextRecord();
                }
            }
        }
    }
}