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


        object  GetBooknosum(string bookdate, string ptype);


      object GetMonthofDay(string StartTime, string type);

        //bool UpdateBooknoTotalTj(List<PersonSum> pdData, ref string error);
        bool UpdateBooknoTotalTj(List<PersonSum> pdData, ref string error);

    }
}
