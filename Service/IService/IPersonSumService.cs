using DataModel;
using DataModel.Other;
using System;
using System.Collections.Generic;

namespace Service.IService
{
    public interface IPersonSumService:IBaseService<PersonSum>
    {
        /// <summary>
        /// 获取个人号源
        /// </summary>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        object GetPersonSumList(string type, int start, int end);
        /// <summary>
        /// 号源修改
        /// </summary>
        /// <param name="date_Time">日期</param>
        /// <param name="personSum_Code">时段code</param>
        /// <param name="type">体检类型</param>
        /// <returns></returns>
        bool PersonSumUpDate(DateTime date_Time, string personSum_Code, string type,ref string error);

        object  GetBooknosum(string bookdate, string ptype);


      object GetMonthofDay(string StartTime, string type);

        //bool UpdateBooknoTotalTj(List<PersonSum> pdData, ref string error);
        bool UpdateBooknoTotalTj(List<PersonSum> pdData, ref string error);

    }
}
