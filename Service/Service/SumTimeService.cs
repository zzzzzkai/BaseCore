using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Linq;

namespace Service.Service
{
    public class SumTimeService:BaseService<SumTime>,ISumTimeService
    {
        private readonly ISumTimeRepository _sumTimeRepository;
        private readonly IPersonSumRepository _personSumRepository;
        public SumTimeService(ISumTimeRepository sumTimeRepository,IPersonSumRepository personSumRepository ) {
            _sumTimeRepository = sumTimeRepository;
            _personSumRepository = personSumRepository;
        }
        /// <summary>
        /// 获取个人时段号源
        /// </summary>
        /// <param name="date_Time"></param>
        /// <returns></returns>
        public object GetSumTimePerson(DateTime date_Time,string type) {
            try
            {            
                var sumList = (from a in _personSumRepository.FindListByClause(x=>x.person_Date==date_Time&& x.person_Type== type)
                               join b in _sumTimeRepository.FindListByClause(z=>z.sumtime_Flag==type)
                               on a.person_Code equals b.sumtime_Code
                               where a.person_Date == date_Time && a.person_Type==b.sumtime_Flag
                               select new 
                               {
                                   b.sumtime_Code,
                                   b.sumtime_Name,
                                   b.sumtime_BegTime,
                                   b.sumtime_EndTime,
                                   a.person_Sum,
                                   a.person_Surplus,
                                   a.person_Already
                               }).ToList();
                return sumList;
            }
            catch (Exception e)
            {

                throw e;
            }
           
        }


    }
}
