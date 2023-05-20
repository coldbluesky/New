using System;
using System.Collections.Generic;
using System.Data;
using Zhaoxi.DigitaPlatform.Entities;

namespace Zhaoxi.DigitaPlatform.IDataAccess
{
    public interface ILocalDataAccess
    {
        DataTable Login(string username, string password);
        void ResetPassword(string username);

        void SaveDevice(List<DeviceEntity> devices);
        List<DeviceEntity> GetDevices();


        List<PropEntity> GetPropertyOption();
        List<ThumbEntity> GetThumbList();


        List<AlarmEntity> GetAlarmList(string condition);
        int SaveAlarmMessage(AlarmEntity alarm);
        int UpdateAlarmState(string aNum, string solveTime);


        void SaveTrend(List<TrendEntity> trends);
        List<TrendEntity> GetTrends();


        void SaveRecord(List<RecordWriteEntity> records);
        List<RecordReadEntity> GetRecords();


        void GetBaseInfo(List<BaseInfoEntity> baseInfos, List<UserEntity> users);
        void SaveBaseInfo(List<BaseInfoEntity> baseInfo, List<UserEntity> users);
    }
}
