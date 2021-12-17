using DataModel;
using DataModel.Other;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Service
{
    public class PersonSumService : BaseService<PersonSum>, IPersonSumService
    {

        private readonly IPersonSumRepository _personSumRepository;
        private readonly ISumTimeRepository _sumTimeRepository;
        public PersonSumService(IPersonSumRepository personSumRepository,
            ISumTimeRepository sumTimeRepository)
        {
            _personSumRepository = personSumRepository;
            _baseRepository = _personSumRepository;
            _sumTimeRepository = sumTimeRepository;
        }
        #region 个人号源（无时段）
        ///// <summary>
        ///// 获取个人号源（无时段）
        ///// </summary>
        ///// <param name="type">个人号源类型</param>
        ///// <param name="start">查询开始时间</param>
        ///// <param name="end">结束时间</param>
        ///// <returns></returns>
        //public object GetPersonSumList(string type, int start, int end)
        //{
        //    try
        //    {
        //        var startDay = DateTime.Now.AddDays(start).ToString("yyyy-MM-dd");
        //        var endDay = DateTime.Now.AddDays(end).ToString("yyyy-MM-dd");
        //        DateTime st = Convert.ToDateTime(startDay);
        //        DateTime et = Convert.ToDateTime(endDay);
        //        var SumList = _personSumRepository.FindListByClause(x => x.person_Date >= st && x.person_Date <= et && x.person_Type == type, "person_Date").ToList()
        //            .Select(z=>new {
        //            person_Date = z.person_Date.Value.ToString("yyyy-MM-dd"),//号源日期
        //            z.id,//id
        //            z.person_Flag,//休假
        //            z.person_Sum,//设定号源
        //            z.person_Surplus,//剩余号源
        //            z.person_Already,//已约号源
        //        }).ToList();
        //        return SumList;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}
        #endregion


        #region 个人号源（时段）
        /// <summary>
        /// 获取个人号源（号源时段）
        /// </summary>
        /// <param name="type">个人号源类型</param>
        /// <param name="start">查询开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public object GetPersonSumList(string type, int start, int end)
        {
            try
            {
                var startDay = DateTime.Now.AddDays(start).ToString("yyyy-MM-dd");
                var endDay = DateTime.Now.AddDays(end).ToString("yyyy-MM-dd");
                DateTime st = Convert.ToDateTime(startDay);
                DateTime et = Convert.ToDateTime(endDay);
                var SumList = _personSumRepository.FindListByClause(x => x.person_Date >= st && x.person_Date <= et && x.person_Type == type)
                    .GroupBy(y => new { y.person_Date, y.person_Flag }).Select(z =>
                        new
                        {
                            person_Date = z.Key.person_Date.Value.ToString("yyyy-MM-dd"),//号源日期
                            person_Sum = z.Sum(a => a.person_Sum),//设定号源
                            person_Surplus = z.Sum(a => a.person_Surplus),//剩余号源
                            person_Already = z.Sum(a => a.person_Already),//已约号源
                            z.Key.person_Flag//休假日期
                        }).ToList();
                return SumList;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public object GetBooknosum(string bookdate, string ptype)
        {
            try
            {
                DateTime dt = new DateTime();
                string team_Flagg = "F";
                DateTime.TryParse(bookdate, out dt);
                string shift = Convert.ToDateTime(dt).DayOfWeek.ToString();
                if (shift == "Sunday")
                {
                    team_Flagg = "T";

                }
                var bookNoList = from t in _sumTimeRepository.FindListByClause(x => x.sumtime_Flag == ptype)
                                 join p in _personSumRepository.FindListByClause(x => x.person_Type == ptype && x.person_Date == dt)
                                 on t.sumtime_Code equals p.person_Code into temp
                                 from tp in temp.DefaultIfEmpty().ToList()
                                 select new
                                 {
                                     t.sumtime_Code,
                                     t.sumtime_BegTime,
                                     t.sumtime_EndTime,
                                     t.sumtime_Name,
                                     person_Date = tp?.person_Date ?? dt,
                                     person_Sum = tp?.person_Sum ?? 0,
                                     person_Already = tp?.person_Already ?? 0,
                                     person_Surplus = tp?.person_Surplus ?? 0,
                                     person_Flag = tp?.person_Flag ?? team_Flagg
                                 };
                return bookNoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public object GetBooknosum(string bookdate, string ptype)
        //{
        //    try
        //    {
        //        DateTime dt = new DateTime();
        //        DateTime.TryParse(bookdate, out dt);
        //        var sumList = (from a in _personSumRepository.FindListByClause(x => x.person_Date == dt && x.person_Type == ptype)
        //                       join b in _sumTimeRepository.FindListByClause(z => z.sumtime_Flag == ptype)
        //                       on a.person_Code equals b.sumtime_Code
        //                       //where a.person_Date == dt && a.person_Type == b.sumtime_Flag
        //                       select new
        //                       {
        //                           b.sumtime_Code,
        //                           b.sumtime_Name,
        //                           b.sumtime_BegTime,
        //                           b.sumtime_EndTime,
        //                           a.person_Sum,
        //                           a.person_Surplus,
        //                           a.person_Already
        //                       }).ToList();
          
        //        return sumList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //} 

      

        public object  GetMonthofDay(string StartTime, string type)
        {
            List<PersonSum> list = new List<PersonSum>();
            var datetime = Convert.ToDateTime(StartTime);
            var first = datetime.AddDays(1 - datetime.Day);
            var last = datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
            var ptype = type;
            var num = DateTime.DaysInMonth(datetime.Year, datetime.Month);
            var query = _personSumRepository.FindListByClause(x => x.person_Date >= first && x.person_Date <= last && x.person_Type == ptype);
            for(int i = 0; i < num; i++)
            {
                var dayNo = query.Where(x => x.person_Date == first.Date.AddDays(i)).Count();
                if (dayNo > 0)
                {
                    if (dayNo > 1)
                    {
                        var querys = query.Where(x => x.person_Date == first.Date.AddDays(i)).ToList() ;
                        PersonSum person = new PersonSum();
                        var person_Sum = 0;
                        var person_Already = 0;
                        var person_Surplus = 0;
                        foreach (var item in querys)
                        {
                           
                            person.person_Date = item.person_Date;
                            person_Sum += Convert.ToInt32(item.person_Sum);
                            person_Already += Convert.ToInt32(item.person_Already);
                            person_Surplus += Convert.ToInt32(item.person_Surplus);
                            person.person_Flag = item.person_Flag;
                 
                        }
                        person.person_Sum = person_Sum;
                        person.person_Surplus = person_Surplus;
                        person.person_Already = person_Already;
                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        switch (shift)
                        {
                            case "Monday":
                                person.person_Week = "周一";
                                break;
                            case "Tuesday":
                                person.person_Week = "周二";
                                break;
                            case "Wednesday":
                                person.person_Week = "周三";
                                break;
                            case "Thursday":
                                person.person_Week = "周四";
                                break;
                            case "Friday":
                                person.person_Week = "周五";
                                break;
                            case "Saturday":
                                person.person_Week = "周六";
                                break;
                            case "Sunday":
                                person.person_Week = "周日";
                                break;
                            default:
                                break;
                        }
                        list.Add(person);
                    }
                    else
                    {
                        var querys = query.Where(x => x.person_Date == first.Date.AddDays(i)).FirstOrDefault();
                        PersonSum person = new PersonSum();

                        person.person_Date = querys.person_Date;
                        person.person_Already = querys.person_Already;
                        person.person_Flag = querys.person_Flag;
                        person.person_Sum = querys.person_Sum;
                        person.person_Surplus = querys.person_Sum;
                        string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                        switch (shift)
                        {
                            case "Monday":
                                person.person_Week = "周一";
                                break;
                            case "Tuesday":
                                person.person_Week = "周二";
                                break;
                            case "Wednesday":
                                person.person_Week = "周三";
                                break;
                            case "Thursday":
                                person.person_Week = "周四";
                                break;
                            case "Friday":
                                person.person_Week = "周五";
                                break;
                            case "Saturday":
                                person.person_Week = "周六";
                                break;
                            case "Sunday":
                                person.person_Week = "周日";
                                break;
                            default:
                                break;
                        }
                        list.Add(person);

                    }
                }
                else
                {
                    PersonSum person = new PersonSum();
                    person.person_Date = first.Date.AddDays(i);
                    person.person_Sum = 0;
                    person.person_Surplus = 0;
                    person.person_Already = 0;
      
                    string shift = first.Date.AddDays(i).DayOfWeek.ToString();
                    switch (shift)
                    {
                        case "Monday":
                            person.person_Week = "周一";
                            break;
                        case "Tuesday":
                            person.person_Week = "周二";
                            break;
                        case "Wednesday":
                            person.person_Week = "周三";
                            break;
                        case "Thursday":
                            person.person_Week = "周四";
                            break;
                        case "Friday":
                            person.person_Week = "周五";
                            break;
                        case "Saturday":
                            person.person_Week = "周六";
                            break;
                        case "Sunday":
                            person.person_Week = "周日";
                            break;
                        default:
                            break;
                    }
                    if (person.person_Week == "周日")
                    {
                        person.person_Flag = "T";
                    }
                    list.Add(person);

                }
            
            }; 

            return list;
            #endregion
        }

        public bool UpdateBooknoTotalTj(List<PersonSum> pdData, ref string error) {
            error = string.Empty;
            try
            {
                foreach (var item in pdData)
                {
                    DateTime times = Convert.ToDateTime(item.StartTime);
                    int nums = Convert.ToDateTime(item.EndTime).Subtract(Convert.ToDateTime(item.StartTime)).Days;
                    for (int i = 0; i <= nums; i++)
                        {
                            var bookdate = Convert.ToDateTime(times.AddDays(i).ToString("yyyy-MM-dd"));
                            var result = _personSumRepository.FindByClause(x => x.person_Date == bookdate && x.person_Type == item.person_Type && x.person_Code == item.person_Code);
                            if (result == null)
                            {
                                PersonSum sum = new PersonSum();
                                sum.person_Sum = item.person_Sum;
                                sum.person_Date = bookdate;
                                sum.person_Surplus = item.person_Sum;
                                sum.person_Type = item.person_Type;
                                sum.person_Already = 0;
                                sum.person_Code = item.person_Code;
                                sum.person_Flag = item.person_Flag;
                                string shift = Convert.ToDateTime(bookdate).DayOfWeek.ToString();
                                switch (shift)
                                {
                                case "Monday":
                                    sum.person_Week = "1";
                                    break;
                                case "Tuesday":
                                    sum.person_Week = "2";
                                    break;
                                case "Wednesday":
                                    sum.person_Week = "3";
                                    break;
                                case "Thursday":
                                    sum.person_Week = "4";
                                    break;
                                case "Friday":
                                    sum.person_Week = "5";
                                    break;
                                case "Saturday":
                                    sum.person_Week = "6";
                                    break;
                                case "Sunday":
                                    sum.person_Week = "7";
                                    break;
                                default:
                                    break;
                                }
                            if (sum.person_Week == "7"&&nums>0)
                            {
                                continue;
                            }
                            else { 
                               _personSumRepository.Insert(sum);
                            
                            }
                        }
                        else
                        {
                            if (result.person_Flag == "T" && nums > 0)
                            {
                                continue;
                            }
                            if (item.person_Flag  == "T")
                            {
                                result.person_Flag = item.person_Flag;
                            }
                            else
                            {
                                result.person_Flag =item.person_Flag;
                                result.person_Surplus += item.person_Sum - result.person_Sum;
                                result.person_Sum = item.person_Sum;
                            }
                            _personSumRepository.Update(result);
                        }                       
                    }                    
                }                  
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("个人后台号源修改失败", ex.Message);
                error = ex.Message;
                return false;
            }
        }



    }
}

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  