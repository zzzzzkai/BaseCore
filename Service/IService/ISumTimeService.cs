using DataModel;
using System;

namespace Service.IService
{
    public interface ISumTimeService:IBaseService<SumTime>
    {
        /// <summary>
        /// 获取个人时段号源
        /// </summary>
        /// <param name="date_Time"></param>
        /// <returns></returns>
        object GetSumTimePerson(DateTime date_Time, string type);

        /// <summary>
        /// 获取团体时段号源
        /// </summary>
        /// <param name="date_Time"></param>
        /// <returns></returns>
        object GetSumTimeTeam(DateTime date_Time,string team_LncCode);

    }
}
