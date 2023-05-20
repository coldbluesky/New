﻿using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Data;

namespace ConsoleApp
{

    
    internal class Program
    {



        static void Main(string[] args)
        {
            string json = @"[{
				""OperationName"": ""SetBakingTemperature"",
	""OperationDesc"": ""温度设定值"",
	""OperationValues"": ""0""
}, {
				""OperationName"": ""TempAlarmOffset"",
	""OperationDesc"": ""温度报警偏移值"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""UpperVacuumPressure"",
	""OperationDesc"": ""真空上限压力"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""LowerVacuumPressure"",
	""OperationDesc"": ""真空下限压力"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""UpperTrogenPressure"",
	""OperationDesc"": ""氮气上限压力"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""LowerTrogenPressure"",
	""OperationDesc"": ""氮气下限压力"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep1"",
	""OperationDesc"": ""流程步1加热开启"",
	""OperationValues"": ""1""
		 }, {
				""OperationName"": ""HeatStep2"",
	""OperationDesc"": ""流程步2加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep3"",
	""OperationDesc"": ""流程步3加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep4"",
	""OperationDesc"": ""流程步4加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep5"",
	""OperationDesc"": ""流程步5加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep6"",
	""OperationDesc"": ""流程步6加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep7"",
	""OperationDesc"": ""流程步7加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep8"",
	""OperationDesc"": ""流程步8加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep9"",
	""OperationDesc"": ""流程步9加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep10"",
	""OperationDesc"": ""流程步10加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep11"",
	""OperationDesc"": ""流程步11加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep12"",
	""OperationDesc"": ""流程步12加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep13"",
	""OperationDesc"": ""流程步13加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep14"",
	""OperationDesc"": ""流程步14加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep15"",
	""OperationDesc"": ""流程步15加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep16"",
	""OperationDesc"": ""流程步16加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep17"",
	""OperationDesc"": ""流程步17加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep18"",
	""OperationDesc"": ""流程步18加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep19"",
	""OperationDesc"": ""流程步19加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""HeatStep20"",
	""OperationDesc"": ""流程步20加热开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep1"",
	""OperationDesc"": ""流程步1抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep2"",
	""OperationDesc"": ""流程步2抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep3"",
	""OperationDesc"": ""流程步3抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep4"",
	""OperationDesc"": ""流程步4抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep5"",
	""OperationDesc"": ""流程步5抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep6"",
	""OperationDesc"": ""流程步6抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep7"",
	""OperationDesc"": ""流程步7抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep8"",
	""OperationDesc"": ""流程步8抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep9"",
	""OperationDesc"": ""流程步9抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep10"",
	""OperationDesc"": ""流程步10抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep11"",
	""OperationDesc"": ""流程步11抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep12"",
	""OperationDesc"": ""流程步12抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep13"",
	""OperationDesc"": ""流程步13抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep14"",
	""OperationDesc"": ""流程步14抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep15"",
	""OperationDesc"": ""流程步15抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep16"",
	""OperationDesc"": ""流程步16抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep17"",
	""OperationDesc"": ""流程步17抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep18"",
	""OperationDesc"": ""流程步18抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep19"",
	""OperationDesc"": ""流程步19抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""VacuumStep20"",
	""OperationDesc"": ""流程步20抽真空开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep1"",
	""OperationDesc"": ""流程步1充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep2"",
	""OperationDesc"": ""流程步2充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep3"",
	""OperationDesc"": ""流程步3充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep4"",
	""OperationDesc"": ""流程步4充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep5"",
	""OperationDesc"": ""流程步5充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep6"",
	""OperationDesc"": ""流程步6充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep7"",
	""OperationDesc"": ""流程步7充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep8"",
	""OperationDesc"": ""流程步8充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep9"",
	""OperationDesc"": ""流程步9充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep10"",
	""OperationDesc"": ""流程步10充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep11"",
	""OperationDesc"": ""流程步11充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep12"",
	""OperationDesc"": ""流程步12充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep13"",
	""OperationDesc"": ""流程步13充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep14"",
	""OperationDesc"": ""流程步14充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep15"",
	""OperationDesc"": ""流程步15充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep16"",
	""OperationDesc"": ""流程步16充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep17"",
	""OperationDesc"": ""流程步17充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep18"",
	""OperationDesc"": ""流程步18充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep19"",
	""OperationDesc"": ""流程步19充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NitrogenStep20"",
	""OperationDesc"": ""流程步20充氮气开启"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep1"",
	""OperationDesc"": ""流程步1工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep2"",
	""OperationDesc"": ""流程步2工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep3"",
	""OperationDesc"": ""流程步3工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep4"",
	""OperationDesc"": ""流程步4工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep5"",
	""OperationDesc"": ""流程步5工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep6"",
	""OperationDesc"": ""流程步6工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep7"",
	""OperationDesc"": ""流程步7工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep8"",
	""OperationDesc"": ""流程步8工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep9"",
	""OperationDesc"": ""流程步9工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep10"",
	""OperationDesc"": ""流程步10工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep11"",
	""OperationDesc"": ""流程步11工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep12"",
	""OperationDesc"": ""流程步12工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep13"",
	""OperationDesc"": ""流程步13工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep14"",
	""OperationDesc"": ""流程步14工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep15"",
	""OperationDesc"": ""流程步15工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep16"",
	""OperationDesc"": ""流程步16工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep17"",
	""OperationDesc"": ""流程步17工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep18"",
	""OperationDesc"": ""流程步18工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep19"",
	""OperationDesc"": ""流程步19工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""ProcessTimeStep20"",
	""OperationDesc"": ""流程步20工艺时间"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleStart1"",
	""OperationDesc"": ""流程步循环1起始步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleStart2"",
	""OperationDesc"": ""流程步循环2起始步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleStart3"",
	""OperationDesc"": ""流程步循环3起始步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleEnd1"",
	""OperationDesc"": ""流程步循环1终止步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleEnd2"",
	""OperationDesc"": ""流程步循环2终止步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""CycleEnd3"",
	""OperationDesc"": ""流程步循环3终止步"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NumberOfCycles1"",
	""OperationDesc"": ""流程步循环1次数"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NumberOfCycles2"",
	""OperationDesc"": ""流程步循环2次数"",
	""OperationValues"": ""0""
		 }, {
				""OperationName"": ""NumberOfCycles3"",
	""OperationDesc"": ""流程步循环3次数"",
	""OperationValues"": ""0""
		 }]";

			JArray jsonArray = JArray.Parse(json);

            string filePath = @"C:\Users\DELL\desktop\generated_code.cs";

            StreamWriter sw = new StreamWriter(filePath, true);
            foreach (JObject obj in jsonArray)
            {
                string operationName = (string)obj["OperationName"];
                string generatedCode  = $"public string {operationName} {{ get; set; }}";
                sw.WriteLine(generatedCode);

            }
            sw.Flush();
            sw.Close();
            // 将生成的代码写入文本文件

        }
    }
}
