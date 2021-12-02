using DataModel;
using System;
using System.Collections.Generic;

namespace Service.IService
{
    public interface ITeamSumService:IBaseService<TeamSum>
    {
        /// <summary>
        /// 查询团体号源
        /// </summary>
        /// <param name="lnccode"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        object GetTeamSumList(string lnccode, int start, int end);
        /// <summary>
        /// 团体号源修改
        /// </summary>
        /// <param name="date_Time"></param>
        /// <param name="lnc_code"></param>
        /// <returns></returns>
        bool TeamSumUpdate(DateTime date_Time, string lnc_code, string sumtime_Code);

        Object GetTeamSumnosumm(string team_Date,string  team_LncCode,string type);

        List<TeamSum> GetMonthofDayT( string date_Time);

        List<TeamSum> GetMonthofDaylnccodeT(string date_Time,string lnccode);
        object  Getlncmenu(string Keyvalue);

        bool UpdateBooknoTotalTj(List<TeamSum> pData, ref string error);

        List<SumTime> Timeslot(SumTime e);


    }
}
